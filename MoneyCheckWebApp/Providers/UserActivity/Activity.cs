#nullable disable

using System;

namespace MoneyCheckWebApp.Providers.UserActivity
{
    public class Activity
    {
        public long Id { get; set; }
        public ActivityType ActivityType { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public DateTime TimeStamp { get; set; }
        
        public decimal Amount { get; set; }
    }

    public enum ActivityType
    {
        BalanceAdd,
        BalanceMinus,
        Purchase,
        DebtAdd,
        DebtMinus
    }
}