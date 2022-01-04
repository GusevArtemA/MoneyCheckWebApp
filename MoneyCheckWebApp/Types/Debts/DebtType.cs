using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyCheckWebApp.Types.Debtors;

namespace MoneyCheckWebApp.Types.Debts
{
    public class DebtType
    {
        public long? DebtId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public long? PurchaseId { get; set; }
        public DebtorType Debtor { get; set; } = null!;
    }
}
