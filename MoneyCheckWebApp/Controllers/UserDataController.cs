using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.UserData;

namespace MoneyCheckWebApp.Controllers
{
    /// <summary>
    /// Контроллер, который предоставляет базовые данные о пользователе
    /// </summary>
    [ApiController]
    [Route("/api/user-data")]
    public class UserDataController : ControllerBase
    {
        /// <summary>
        /// Получает базовые данные
        /// </summary>
        /// <returns></returns>
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