using System;
using System.Linq;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Predications.LinearRegression
{
    public class PurchasesDecimalArrayProvider : IDecimalArrayProvider
    {
        private readonly User _target;

        public PurchasesDecimalArrayProvider(User target)
        {
            _target = target;
        }
        
        public decimal[] ProvideArray()
        {
            try
            {
                var monthStats = _target.Purchases.Where(x => x.BoughtAt.Year == DateTime.Now.Year && x.Category.CategoryName != "Зачисление")
                    .GroupBy(x => x.BoughtAt.Month)
                    .Select(x => x.Select(y => y.Amount)
                        .Sum())
                    .ToArray();

                return monthStats.Length == 0 ? Array.Empty<decimal>() : monthStats;
            }
            catch (InvalidOperationException)
            {
                return Array.Empty<decimal>();
            }
        }
    }
}