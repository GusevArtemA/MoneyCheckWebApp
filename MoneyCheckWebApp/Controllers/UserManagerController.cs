using System;
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

        [HttpPatch]
        [Route("update-balance")]
        public async Task<IActionResult> UpdateBalance(decimal? actual, decimal? delta)
        {
            var user = this.ExtractUser();
            if (actual == null && delta == null || actual != null && delta != null)
            {
                return BadRequest();
            }

            var update = new AccountBalanceUpdate()
            {
                Amount = actual ?? delta!.Value,
            };

            if (actual != null)
            {
                update.OperationType = 0;
                user.Balance = actual.Value;
            }

            if (delta != null)
            {
                update.OperationType = 1;
                user.Balance += delta.Value;
            }

            update.UpdateAt = DateTime.Now;
            _context.AccountBalanceUpdates.Add(update);

            update.User = user;
            
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}