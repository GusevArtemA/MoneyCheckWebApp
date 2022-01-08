using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Helpers;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Pipelines;
using MoneyCheckWebApp.Services;
using MoneyCheckWebApp.Types.Auth;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("auth/api")]
    public class AuthController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;
        private readonly CookieService _cookieService;
        private readonly AuthorizationService _authService;

        private const int CookieLifetime = 259200; //Время жизни cookie файлов в секундах

        public AuthController(MoneyCheckDbContext context, CookieService cookieService, AuthorizationService authService)
        {
            _context = context;
            _cookieService = cookieService;
            _authService = authService;
        }
        
        /// <summary>
        /// Генерирует токен и отправляет его в виде cookie файлов с сроком жизни 259200 секунд 
        /// </summary>
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(
            [FromBody]
            LoginType login)
        {
            if (!Request.IsHttps)
            {
                return BadRequest(Statuses.HttpsRequiredStatus);
            }
            
            var firstAssociatedUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == login.Username);
            
            if (firstAssociatedUser == null)
            {
                return Unauthorized();
            }

            if (firstAssociatedUser.PasswordMd5Hash != login.PasswordHash)
            {
                return Unauthorized();
            }

            var pipeline = new AuthorizationCookieConfigurationPipeline();
            
            var token = await pipeline.ExecutePipelineAsync(firstAssociatedUser, Request, Response, _cookieService, _authService,
                CookieLifetime);

            return Ok(new TokenReplyType
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt
            });
        }

        /// <summary>
        /// Выполняет логаут для текущего токена
        /// </summary>
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> Logout(string token)
        {
            if (!Request.IsHttps)
            {
                return BadRequest(Statuses.HttpsRequiredStatus);
            }
            
            var firstToken = await _context.UserAuthTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (firstToken == null)
            {
                return Unauthorized();
            }

            _context.UserAuthTokens.Remove(firstToken);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Производит регистрацию пользователя
        /// </summary>
        /// <returns>Токен и куки, если успешно</returns>
        [HttpPost]
        [Route("log-up")]
        public async Task<IActionResult> LogUp(
            [FromBody]
            LogUpType logUp)
        {
            if (!Request.IsHttps)
            {
                return BadRequest(Statuses.HttpsRequiredStatus);
            }
            
            var username = logUp.Username;

            if (await _context.Users.AnyAsync(x => x.Username == username))
            {
                return BadRequest(Statuses.UserAlreadyCreatedStatus);
            }
            
            using var md5 = new Md5HashHelper();
            var password = logUp.Password;

            var encoded = md5.Compute(password);

            var user = new User()
            {
                Username = username,
                PasswordMd5Hash = encoded,
                Balance = 0
            };

            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            var pipeline = new AuthorizationCookieConfigurationPipeline();
            
            var token = await pipeline.ExecutePipelineAsync(user, Request, Response, _cookieService, _authService,
                CookieLifetime);

            return Ok(new TokenReplyType
            {
                Token = token.Token,
                ExpiresAt = token.ExpiresAt
            });
        }
    }
}