using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public async Task FillHttpContextAsync(MoneyCheckDbContext context)
        {
            _context.Items.Add("ContextUser", await context.Users.FirstOrDefaultAsync(x => x.Id == _token.UserId));
        }
    }
}