using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Friend
    {
        public long FriendAid { get; set; }
        public long FriendBid { get; set; }

        public virtual User FriendA { get; set; }
        public virtual User FriendB { get; set; }
    }
}
