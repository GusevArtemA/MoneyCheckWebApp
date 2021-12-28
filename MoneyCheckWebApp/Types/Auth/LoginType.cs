#nullable disable

namespace MoneyCheckWebApp.Types.Auth
{
    public class LoginType
    {
        public string Username { get; set; }

        /// <summary>
        /// Md5-хэш пароля
        /// </summary>
        public string PasswordHash { get; set; }
    }
}