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
                var daysStats = _target.Purchases.Where(x =>
                        x.BoughtAt.Year == DateTime.Now.Year &&
                        x.BoughtAt.Month == DateTime.Now.Month &&
                        x.Category.CategoryName != "Зачисление")
                    .GroupBy(x => x.BoughtAt.Day)
                    .Select(x => x.Select(y => y.Amount)
                        .Sum())
                    .ToArray();

                decimal[] daysStatsExtended = new decimal[daysStats.Length];
                
                for (var i = 0; i < daysStats.Length; i++)
                {
                    var sum = daysStats[i];
                    
                    for (var i1 = 0; i1 < i; i1++)
                    {
                        sum += daysStats[i1];
                    }

                    daysStatsExtended[i] = sum;
                }
                
                return daysStatsExtended;
            }
            catch (InvalidOperationException)
            {
                return Array.Empty<decimal>();
            }
        }
    }
}