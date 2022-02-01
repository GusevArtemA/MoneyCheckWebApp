using System;

namespace MoneyCheckWebApp.ExportingServices.Csv.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvPropertyNameAttribute : Attribute
    {
        public CsvPropertyNameAttribute(string name)
        {
            Name = name;
        }
        
        public string Name { get; }
    }
}