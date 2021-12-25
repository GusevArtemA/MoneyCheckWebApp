using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Category
    {
        public Category()
        {
            ShopItems = new HashSet<ShopItem>();
        }

        public long Id { get; set; }
        public string CategoryName { get; set; }

        public virtual ICollection<ShopItem> ShopItems { get; set; }
    }
}
