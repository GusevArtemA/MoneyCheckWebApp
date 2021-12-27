using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Helpers;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/user-manager")]
    public class UserManagerController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public UserManagerController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Изменяет хэш пароля в базе данных
        /// </summary>
        [HttpPatch]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword(string password)
        {
            using var md5 = new Md5HashHelper();
            var user = this.ExtractUser();

            var hash = md5.Compute(password);

            if (user.PasswordMd5Hash == hash)
            {
                return BadRequest("New password should be differ from current password");
            }

            user.PasswordMd5Hash = hash;

            await _context.SaveChangesAsync();
            
            return Ok();
        }
    }
}