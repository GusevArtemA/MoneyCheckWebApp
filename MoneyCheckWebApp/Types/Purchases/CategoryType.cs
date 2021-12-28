using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCheckWebApp.Types.Purchases
{
    public class CategoryType
    {
        public string CategoryName { get; set; }
        public long? ParentCategoryId { get; set; }
    }
}
