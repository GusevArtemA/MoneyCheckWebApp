#nullable disable

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MoneyCheckWebApp.Types.UserStats
{
    public class CategoryDataType
    {
        public long Id { get; set; }
        
        public string CategoryName { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<CategoryDataType>? ChildCategories { get; set; }
        
        public decimal CategoryAmount { get; set; }
    }
}