using System;

namespace MoneyCheckWebApp.Types.Auth
{
    public class TokenReplyType
    {
        public string Token { get; set; }
        
        public DateTime ExpiresAt { get; set; }
    }
}