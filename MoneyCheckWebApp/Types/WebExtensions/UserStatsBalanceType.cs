namespace MoneyCheckWebApp.Types.WebExtensions
{
    public class UserStatsBalanceType
    {
        public decimal Balance { get; set; }
        public decimal TodaySpent { get; set; }
        public decimal FutureCash { get; set; }
        public double InflationCash { get; set; }
    }
}