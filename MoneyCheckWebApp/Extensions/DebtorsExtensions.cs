using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debtors;

namespace MoneyCheckWebApp.Extensions
{
    public static class DebtorsExtensions
    {
        public static DebtorType GenerateApiType(this Debtor debtor) => new DebtorType
        {
            Name = debtor.Name,
            NaturalMaskId = debtor.NaturalMaskId
        };
    }
}