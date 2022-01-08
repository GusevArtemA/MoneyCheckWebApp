using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Debt
    {
        public Debt()
        {
            DebtUpdates = new HashSet<DebtUpdate>();
        }

        public long DebtorId { get; set; }
        public long? PurchaseId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long DebtId { get; set; }
        public long? InitiatorId { get; set; }

        public virtual Debtor Debtor { get; set; }
        public virtual User Initiator { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual ICollection<DebtUpdate> DebtUpdates { get; set; }
    }
}
