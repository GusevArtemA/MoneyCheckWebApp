using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Debtor
    {
        public Debtor()
        {
            Debts = new HashSet<Debt>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long? NaturalMaskId { get; set; }

        public virtual User NaturalMask { get; set; }
        public virtual ICollection<Debt> Debts { get; set; }
    }
}
