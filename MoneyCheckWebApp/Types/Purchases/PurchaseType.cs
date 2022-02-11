#nullable disable

using System;
using MoneyCheckWebApp.ExportingServices.Csv.Attributes;

namespace MoneyCheckWebApp.Types.Purchases
{
    public class PurchaseType
    {
        public long Id { get; set; }
        
        [CsvPropertyName("Время и дата покупки")]
        public DateTime BoughtAt { get; set; }
        
        [CsvPropertyName("Цена")]
        public decimal Amount { get; set; }
        
        [CsvPropertyName("Id категории")]
        public long CategoryId { get; set; }
        
        [CsvPropertyName("Категория")]
        public string CategoryName { get; set; }
        
        [CsvPropertyName("Долгота")]
        public decimal? Longitude { get; set; }
        
        [CsvPropertyName("Широта")]
        public decimal? Latitude { get; set; }
    }
    
    public class EditPurchaseType
    {
        public long Id { get; set; }
        public decimal? Amount { get; set; }
        public long? CategoryId { get; set; }
    }
}
