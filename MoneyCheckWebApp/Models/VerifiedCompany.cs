using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class VerifiedCompany
    {
        public VerifiedCompany()
        {
            Purchases = new HashSet<Purchase>();
        }

        public long Id { get; set; }
        public string CompanyName { get; set; }
        public long CategoryId { get; set; }
        public string LogoSvg { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
