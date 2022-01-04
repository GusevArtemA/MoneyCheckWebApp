#nullable disable

namespace MoneyCheckWebApp.Types.Purchases
{
    public class CategoryType
    {
        public string CategoryName { get; set; }
        public long? ParentCategoryId { get; set; }
    }
}
