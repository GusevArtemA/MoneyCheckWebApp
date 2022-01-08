using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class AccountBalanceUpdate
    {
        public long UpdateId { get; set; }
        public long UserId { get; set; }
        public int OperationType { get; set; }
        public decimal Amount { get; set; }
        public DateTime UpdateAt { get; set; }

        public virtual User User { get; set; }
    }
}
