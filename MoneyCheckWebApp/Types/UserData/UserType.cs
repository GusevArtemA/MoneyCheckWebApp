#nullable disable

namespace MoneyCheckWebApp.Types.UserData
{
    public class UserType
    {
        public string Username { get; set; }
        
        public decimal Balance { get; set; }

        public long Id { get; set; }
    }
}