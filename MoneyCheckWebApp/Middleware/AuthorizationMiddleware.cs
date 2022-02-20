using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.MiddlewareServices;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, MoneyCheckDbContext dbContext)
        {
            var url = context.Request.GetEncodedUrl();
            var urlSplited = url.Split("/");
            var tokenProvider = new ContextTokenFetcher(context.Request);

            var authToken = tokenProvider.Provide();
            
            if (urlSplited.Length < 4 || urlSplited[3] != "api")
            {
                await _next(context);
                return;
            }

            if (authToken == null)
            {
                await _next(context);
                return;
            }

            var associatedToken = await dbContext.UserAuthTokens.FindAsync(authToken);

            if (associatedToken == null)
            {
                await HandleError(context);
                return;
            }
            
            var httpFiller = new HttpContextAuthorizationFiller(context, associatedToken!);
                                            
            httpFiller.FillHttpContext(); 

            await _next(context);
        }
        
        private Task HandleError(HttpContext context) 
        {
            const HttpStatusCode code = HttpStatusCode.Unauthorized;
            string result = JsonSerializer.Serialize(new { error = "Unauthorized" });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }

    public interface ICredentialsProvider
    {
        string? ProvideToken(HttpRequest request);
    }

    public class UrlTokenProvider : ICredentialsProvider
    {
        public string? ProvideToken(HttpRequest request)
        {
            var query = request.Query;

            if (!query.ContainsKey("token"))
            {
                return null;
            }

            var token = query["token"].ToString();

            return token;
        }
    }

    public class CookieTokenProvider : ICredentialsProvider
    {
        public string? ProvideToken(HttpRequest request)
        {
            var cookies = request.Cookies;
            var tok = cookies["cmAuthToken"];

            return tok ?? null;
        }
    }

    public class ContextTokenFetcher
    {
        private readonly HttpRequest _request;
        private readonly ICredentialsProvider _urlProvider;
        private readonly ICredentialsProvider _cookieProvider;
        
        public ContextTokenFetcher(HttpRequest request)
        {
            _request = request;
            _urlProvider = new UrlTokenProvider();
            _cookieProvider = new CookieTokenProvider();
        }

        public string? Provide()
        {
            return _urlProvider.ProvideToken(_request) ?? _cookieProvider.ProvideToken(_request);
        }
    }
}