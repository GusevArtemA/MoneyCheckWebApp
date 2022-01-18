using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debts;

namespace MoneyCheckWebApp.Extensions
{
    public static class DebtorsExtensions
    {
        public static DebtorType GenerateApiType(this Debtor debtor) => new DebtorType
        {
            Name = debtor.Name
        };
    }
}