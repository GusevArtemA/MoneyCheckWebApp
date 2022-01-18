using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable enable

namespace MoneyCheckWebApp.Types.Debts
{
    public class DebtorType
    {
        public string Name { get; set; } = null!;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public long? Id { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<DebtType>? Debts { get; set; }
    }
}
