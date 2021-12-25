using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class Purchase
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public DateTime BoughtAt { get; set; }

        public virtual User Customer { get; set; }
    }
}
