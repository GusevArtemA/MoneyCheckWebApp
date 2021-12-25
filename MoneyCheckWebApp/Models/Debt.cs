using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Debt
    {
        public long Id { get; set; }
        public long? To { get; set; }
        public long? From { get; set; }
        public long? PurchaseId { get; set; }
        public int? DebtSize { get; set; }

        public virtual User FromNavigation { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual User ToNavigation { get; set; }
    }
}
