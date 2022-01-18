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

    public class EditDebtType
    {
        public long? DebtId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public long? PurchaseId { get; set; }
    }
}
