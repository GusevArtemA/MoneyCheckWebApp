using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.UserData;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/user-data")]
    public class UserDataController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public UserDataController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get-data")]
        public IActionResult GetUserInfo()
        {
            var user = this.ExtractUser();

            return Ok(new UserType()
            {
                Username = user.Username,
                Balance = user.Balance,
                Id = user.Id
            });
        }
    }
}