using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Services;

namespace MoneyCheckWebApp.Pipelines
{
    public class AuthorizationCookieConfigurationPipeline
    {
        public async Task<UserAuthToken> ExecutePipelineAsync(User target,
            HttpRequest request,
            HttpResponse response,
            CookieService cookieService,
            AuthorizationService authorizationService,
            int credsLifetime)
        {
            var token = await authorizationService.GenerateTokenAsync(target, credsLifetime);

            cookieService.GenerateAuthCookies(request, response, token, credsLifetime);

            return token;
        }
    }
}