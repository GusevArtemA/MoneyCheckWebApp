using System;
using System.Collections.Generic;

#nullable disable

namespace MoneyCheckWebApp.Models
{
    public partial class DefaultLogosForCategory
    {
        public DefaultLogosForCategory()
        {
            Categories = new HashSet<Category>();
        }

        public long Id { get; set; }
        public string Svg { get; set; }
        public string LogoName { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
