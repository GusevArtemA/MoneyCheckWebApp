using Microsoft.AspNetCore.Builder;
using MoneyCheckWebApp.Middleware;

namespace MoneyCheckWebApp.Extensions
{
    public static class TokenAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthorizationMiddleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<TokenAuthorizationMiddleware>();

            return builder;
        }
    }
}