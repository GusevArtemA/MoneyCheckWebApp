using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Category
    {
        public Category()
        {
            InverseParentCategory = new HashSet<Category>();
            Purchases = new HashSet<Purchase>();
            VerifiedCompanies = new HashSet<VerifiedCompany>();
        }

        public long Id { get; set; }
        public string CategoryName { get; set; }
        public long? ParentCategoryId { get; set; }
        public long? OwnerId { get; set; }
        public long LogoId { get; set; }

        public virtual DefaultLogosForCategory Logo { get; set; }
        public virtual User Owner { get; set; }
        public virtual Category ParentCategory { get; set; }
        public virtual ICollection<Category> InverseParentCategory { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
        public virtual ICollection<VerifiedCompany> VerifiedCompanies { get; set; }
    }
}
