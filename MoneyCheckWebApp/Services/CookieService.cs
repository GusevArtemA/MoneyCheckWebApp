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
        public void GenerateAuthCookies(HttpRequest request, HttpResponse response,
            UserAuthToken token, int cookieLifetime)
        {
            var hostName = new Uri(request.GetEncodedUrl()).Host;
            StringBuilder cookieStringBuilder = new StringBuilder();

            cookieStringBuilder.Append("cmAuthToken=");
            
            cookieStringBuilder.Append(token.Token + $"; Max-Age={cookieLifetime};Domain={hostName}; path=/;");
            
            response.Headers.Add("Set-Cookie", cookieStringBuilder.ToString());
        }

        public void GenerateAuthCookies(ControllerBase controllerBase, UserAuthToken target, int cookieLifetime) =>
            GenerateAuthCookies(controllerBase.Request, controllerBase.Response, target, cookieLifetime);
    }
}