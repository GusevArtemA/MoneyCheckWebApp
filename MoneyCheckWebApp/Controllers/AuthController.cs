using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Services;
using MoneyCheckWebApp.Types.Auth;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("auth/api")]
    public class AuthController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;
        private readonly CookieService _cookieService;

        private const int CookieLifetime = 221184000; //Время жизни cookie файлов в секундах

        public AuthController(MoneyCheckDbContext context, CookieService cookieService)
        {
            _context = context;
            _cookieService = cookieService;
        }
        
        /// <summary>
        /// Генерирует токен и отправляет его в виде cookie файлов с сроком жизни 221184000 секунды 
        /// </summary>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(
            [FromBody]
            LoginType login)
        {
            var firstAssociatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == login.Username);
            
            if (firstAssociatedUser == null)
            {
                return Unauthorized();
            }

            if (firstAssociatedUser.PasswordMd5Hash != login.PasswordHash)
            {
                return Unauthorized();
            }

            var token = await _cookieService.GenerateAuthCookiesAsync(this, firstAssociatedUser, CookieLifetime);

            return Ok(new TokenReplyType
            {
                Token = token.Item1,
                ExpiresAt = token.Item2
            });
        }
    }
}