using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Category
    {
        public Category()
        {
            InverseSubCategory = new HashSet<Category>();
            Purchases = new HashSet<Purchase>();
        }

        public long Id { get; set; }
        public string CategoryName { get; set; }
        public long? SubCategoryId { get; set; }

        public virtual Category SubCategory { get; set; }
        public virtual ICollection<Category> InverseSubCategory { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
