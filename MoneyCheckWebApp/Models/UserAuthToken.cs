using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class UserAuthToken
    {
        public long? UserId { get; set; }
        public string Token { get; set; }

        public virtual User User { get; set; }
    }
}
