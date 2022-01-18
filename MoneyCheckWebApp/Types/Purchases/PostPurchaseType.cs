using System;

namespace MoneyCheckWebApp.Types.Purchases
{
    public class PostPurchaseType
    {
        public DateTime? BoughtAt { get; set; }
        public long Amount { get; set; }
        public long CategoryId { get; set; }
        public long? VerifiedCompanyId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}