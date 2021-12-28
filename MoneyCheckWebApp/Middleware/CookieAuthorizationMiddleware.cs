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
    public class CookieAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CookieAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, MoneyCheckDbContext dbContext)
        {
            var url = context.Request.GetEncodedUrl();
            var urlSplited = url.Split("/");
            if (urlSplited.Length < 4 || urlSplited[3] != "api")
            {
                await _next(context);
                return;
            }

            var cookies = context.Request.Cookies;
            var authToken = cookies["cmAuthToken"];

            if (authToken == null)
            {
                await _next(context);
                return;
            }

            var associatedToken = await dbContext.UserAuthTokens.FirstOrDefaultAsync(x => x.Token == authToken);

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
            var code = HttpStatusCode.Unauthorized;
            string result = JsonSerializer.Serialize(new { error = "Unauthorized" });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}