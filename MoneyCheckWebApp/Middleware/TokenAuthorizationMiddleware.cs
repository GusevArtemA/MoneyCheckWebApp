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
    /// <summary>
    /// Промежуточный слой авторизации по токену, вызываемый  для /secure/* части API
    /// </summary>
    public class TokenAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, MoneyCheckDbContext dbContext)
        {
            var urlSplited = httpContext.Request.GetEncodedUrl().Split("/");
            if (urlSplited.Length >= 4 && urlSplited[3] == "api" && !httpContext.Items.ContainsKey("ContextUser"))
            {
                var query = httpContext.Request.Query;

                if (!query.ContainsKey("token"))
                {
                    await HandleError(httpContext);
                    return;
                }

                var token = query["token"].ToString();

                if (string.IsNullOrEmpty(token))
                {
                    await HandleError(httpContext);
                    return;
                }

                var firstAssociatedToken = await dbContext.UserAuthTokens.FirstOrDefaultAsync(x => x.Token == token);
            
                if(firstAssociatedToken == null) 
                {
                    await HandleError(httpContext);
                    return;
                }

                if (DateTime.Now < firstAssociatedToken.ExpiresAt)
                {
                    var httpFiller = new HttpContextAuthorizationFiller(httpContext, firstAssociatedToken);
                                            
                    httpFiller.FillHttpContext();

                    await _next(httpContext);    
                }
                else
                {
                    await HandleError(httpContext);
                }
            }
            else
            {
                await _next(httpContext);
            }
        }
        
        private Task HandleError(HttpContext context) 
        {
            HttpStatusCode code = HttpStatusCode.Unauthorized;
            string result = JsonSerializer.Serialize(new { error = "Unauthorized" });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}