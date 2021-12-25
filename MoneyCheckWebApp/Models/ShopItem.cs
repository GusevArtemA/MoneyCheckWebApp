using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class ShopItem
    {
        public long Id { get; set; }
        public string ItemName { get; set; }
        public long? ShopId { get; set; }
        public int? ItemType { get; set; }
        public long? Cost { get; set; }

        public virtual Shop Shop { get; set; }
    }
}
