using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class PurchaseShopItemTransfer
    {
        public long ShopItemId { get; set; }
        public long PurchaseId { get; set; }

        public virtual Purchase Purchase { get; set; }
        public virtual ShopItem ShopItem { get; set; }
    }
}
