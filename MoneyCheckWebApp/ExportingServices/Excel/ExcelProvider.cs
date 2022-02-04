using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MoneyCheckWebApp.ExportingServices.Excel.Attributes;
using OfficeOpenXml;

namespace MoneyCheckWebApp.ExportingServices.Excel
{
    public class ExcelProvider<T>
    {
        public byte[] GenerateSheet(IEnumerable<T> generatableSheet, string sheetName)
        {
            var package = new ExcelPackage();
            var headerSetuper = new ExcelHeaderSetuper<T>();
            var sheet = package.Workbook.Worksheets.Add(sheetName);
            var props = typeof(T).GetProperties();
            
            headerSetuper.GenerateHeaders(sheet);

            var arrayed = generatableSheet.ToArray();
            
            for (var i = 0; i < arrayed.Length; i++)
            {
                var context = arrayed[i];
                
                for (var i1 = 0; i1 < props.Length; i1++)
                {
                    sheet.Cells[i + 1, i1 + 1].Value = props[i1].GetValue(context)?.ToString() ?? "-";
                }
            }

            sheet.Protection.IsProtected = true;
            
            return package.GetAsByteArray();
        }
    }

    public class ExcelHeaderSetuper<T>
    {
        public void GenerateHeaders(ExcelWorksheet worksheet)
        {
            var props = typeof(T).GetProperties()
                .Select(x => x.GetCustomAttribute<ExcelPropertyNameAttribute>()?.Name ?? x.Name)
                .ToArray();

            for (var i = 0; i < props.Length; i++)
            {
                worksheet.Cells[i + 1, 1].Value = props[i];
            }
        }
    }
}