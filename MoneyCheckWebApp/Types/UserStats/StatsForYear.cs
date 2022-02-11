#nullable disable

using System.Collections.Generic;

namespace MoneyCheckWebApp.Types.UserStats
{
    public class StatsForYear
    {
        public IEnumerable<StatTrace> Months { get; set; }
    }

    public class StatTrace
    {
        public string Index { get; set; }
        
        public decimal? Amount { get; set; }
    }
}