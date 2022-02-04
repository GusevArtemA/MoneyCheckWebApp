using System;

namespace MoneyCheckWebApp.ExportingServices.Excel.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelPropertyNameAttribute : Attribute
    {
        public string Name { get; }

        public ExcelPropertyNameAttribute(string name)
        {
            Name = name;
        }
    }
}