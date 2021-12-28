using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCheckWebApp.Types.Purchases
{
    public class PurchaseType
    {
        public long Id { get; set; }
        public DateTime BoughtAt { get; set; }
        public long Amount { get; set; }
        public long CategoryId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
    }
}
