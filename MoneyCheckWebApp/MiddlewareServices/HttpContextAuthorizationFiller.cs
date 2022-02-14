using Microsoft.AspNetCore.Http;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.MiddlewareServices
{
    /// <summary>
    /// Предоставляет инструмент для заполнения <see cref="HttpContext"/> с нужными данными с поледующим использованием в котроллерах
    /// </summary>
    public class HttpContextAuthorizationFiller
    {
        private readonly HttpContext _context;
        private readonly UserAuthToken _token;

        public HttpContextAuthorizationFiller(HttpContext context, UserAuthToken token)
        {
            _context = context;
            _token = token;
        }

        public void FillHttpContext()
        {
            _context.Items.Add("ContextUser", _token.User);
        }
    }
}