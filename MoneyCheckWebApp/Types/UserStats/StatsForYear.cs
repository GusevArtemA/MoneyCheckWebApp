using System.Collections;
using System.Collections.Generic;

namespace MoneyCheckWebApp.Types.UserStats
{
    public class StatsForYear
    {
        public IEnumerable<StatForMonth> Months { get; set; }
    }

    public class StatForMonth
    {
        public int Number { get; set; }
        
        public decimal Amount { get; set; }
    }
}