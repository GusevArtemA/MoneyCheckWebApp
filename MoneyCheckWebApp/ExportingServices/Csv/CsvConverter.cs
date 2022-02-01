using System.Collections.Generic;
using System.Text;
using MoneyCheckWebApp.ExportingServices.Csv.Movers;

namespace MoneyCheckWebApp.ExportingServices.Csv
{
    public class CsvConverter<T>
    {
        public string Convert(T item)
        {
            var cursorService = new CursorService(new CsvObjectPropertiesCursorMover(item!));
            return cursorService.ReadToEnd();
        }

        public string ConvertEnumerable(IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            var buffer = new StringBuilder();
            buffer.AppendLine(GetCsvHeader());

            for (;;)
            {
                var move = enumerator.MoveNext();

                if (!move)
                {
                    break;
                }
                
                buffer.AppendLine(Convert(enumerator.Current));
            }

            return buffer.ToString();
        }

        private string GetCsvHeader()
        {
            var cursorService = new CursorService(new CsvObjectMetadataCursorMover<T>());

            return cursorService.ReadToEnd();
        }
    }
}