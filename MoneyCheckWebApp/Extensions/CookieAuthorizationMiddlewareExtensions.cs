using Microsoft.AspNetCore.Builder;
using MoneyCheckWebApp.Middleware;

namespace MoneyCheckWebApp.Extensions
{
    public static class CookieAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCookieAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<CookieAuthorizationMiddleware>();

            return builder;
        }
    }
}