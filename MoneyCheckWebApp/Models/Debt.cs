using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Debt
    {
        public long DebtorId { get; set; }
        public long? PurchaseId { get; set; }
        public long Count { get; set; }
        public string Description { get; set; }
        public long DebtId { get; set; }

        public virtual Debtor Debtor { get; set; }
        public virtual Purchase Purchase { get; set; }
    }
}
