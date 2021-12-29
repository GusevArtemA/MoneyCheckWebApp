using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCheckWebApp.Types.Debts
{
    public class DebtType
    {
        public long? DebtId { get; set; }
        public long Count { get; set; }
        public string? Description { get; set; }
        public long? PurchaseId { get; set; }
        public long DebtorId { get; set; }
    }
}
