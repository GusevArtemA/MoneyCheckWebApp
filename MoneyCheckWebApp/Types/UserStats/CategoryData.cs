using System.Collections.Generic;

namespace MoneyCheckWebApp.Types.UserStats
{
    public class CategoryDataType
    {
        public long Id { get; set; }
        public IEnumerable<CategoryDataType>? ChildCategories { get; set; }
        public decimal CategoryAmount { get; set; }
    }
}