using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Purchase
    {
        public Purchase()
        {
            Debts = new HashSet<Debt>();
        }

        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BoughtAt { get; set; }
        public decimal Amount { get; set; }
        public long CategoryId { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public long? VerifiedCompanyId { get; set; }

        public virtual Category Category { get; set; }
        public virtual User Customer { get; set; }
        public virtual VerifiedCompany VerifiedCompany { get; set; }
        public virtual ICollection<Debt> Debts { get; set; }
    }
}
