using Microsoft.AspNetCore.Builder;
using MoneyCheckWebApp.Middleware;

namespace MoneyCheckWebApp.Extensions
{
    public static class TokenAuthorizationMidllewareExtension
    {
        public static void UseAuthMidlleware(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<AuthorizationMiddleware>();
        }
    }
}