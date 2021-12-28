using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Services
{
    public class CookieService
    {
        private readonly MoneyCheckDbContext _context;

        public CookieService(MoneyCheckDbContext context)
        {
            _context = context;
        }

        public async Task<Tuple<string, DateTime>> GenerateAuthCookiesAsync(HttpRequest request, HttpResponse response, User target, int cookieLifetime)
        {
            var hostName = new Uri(request.GetEncodedUrl()).Host;
            StringBuilder cookieStringBuilder = new StringBuilder();

            cookieStringBuilder.Append("cmAuthToken=");

            var token = new UserAuthToken
            {
                Token = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now + TimeSpan.FromSeconds(cookieLifetime)
            };

            await _context.UserAuthTokens.AddAsync(token);

            token.User = target;

            await _context.SaveChangesAsync();

            cookieStringBuilder.Append(token.Token + $"; Max-Age={cookieLifetime};Domain={hostName}; path=/;");
            
            response.Headers.Add("Set-Cookie", cookieStringBuilder.ToString());

            return new Tuple<string, DateTime>(token.Token, DateTime.Now.AddSeconds(cookieLifetime));
        }

        public async Task<Tuple<string, DateTime>> GenerateAuthCookiesAsync(ControllerBase controllerBase, User target, int cookieLifetime) =>
            await GenerateAuthCookiesAsync(controllerBase.Request, controllerBase.Response, target, cookieLifetime);
    }
}