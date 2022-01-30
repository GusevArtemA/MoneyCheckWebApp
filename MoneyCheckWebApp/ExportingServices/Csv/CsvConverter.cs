using System;
using System.Collections.Generic;
using System.Text;

namespace MoneyCheckWebApp.ExportingServices.Csv
{
    public class CsvConverter<T>
    {
        public string Convert(T item)
        {
            StringBuilder buffer = new();
            
            var cursor = new CsvObjectPropertiesCursorMover(item!);

            for (;;)
            {
                var chunk = cursor.MoveNext();

                if (chunk.End)
                {
                    break;
                }

                buffer.Append(chunk.Result);
            }
            

            return buffer.ToString();
        }

        public string ConvertEnumerable(IEnumerable<T> enumerable)
        {
            using var enumerator = enumerable.GetEnumerator();
            var buffer = new StringBuilder();
            string cachedLine = string.Empty;

            for (;;)
            {
                var move = enumerator.MoveNext();

                if (!move)
                {
                    buffer.Append(cachedLine);
                    break;
                }

                if (!string.IsNullOrEmpty(cachedLine))
                {
                    buffer.AppendLine(cachedLine);    
                }
                
                cachedLine = Convert(enumerator.Current);
            }

            return buffer.ToString();
        }
    }
}