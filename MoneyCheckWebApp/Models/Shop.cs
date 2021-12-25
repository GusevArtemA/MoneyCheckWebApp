using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Shop
    {
        public Shop()
        {
            ShopItems = new HashSet<ShopItem>();
        }

        public long Id { get; set; }
        public string ShopName { get; set; }

        public virtual ICollection<ShopItem> ShopItems { get; set; }
    }
}
