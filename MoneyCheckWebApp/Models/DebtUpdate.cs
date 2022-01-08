using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class DebtUpdate
    {
        public long UpdateId { get; set; }
        public long DebtId { get; set; }
        public decimal Amount { get; set; }
        public DateTime UpdateAt { get; set; }

        public virtual Debt Debt { get; set; }
    }
}
