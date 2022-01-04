#nullable disable

namespace MoneyCheckWebApp.Types.Debts
{
    public class DebtorCollectorType
    {
        public string Name { get; set; }
        
        public long? NaturalMaskId { get; set; }

        public DebtType[] Debts { get; set; }
    }
}