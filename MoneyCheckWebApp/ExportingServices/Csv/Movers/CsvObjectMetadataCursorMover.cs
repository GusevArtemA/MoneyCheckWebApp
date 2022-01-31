using System.Reflection;
using System.Text;
using MoneyCheckWebApp.ExportingServices.Csv.Attributes;

namespace MoneyCheckWebApp.ExportingServices.Csv.Movers
{
    public class CsvObjectMetadataCursorMover<T> : ICursorMover
    {
        private int _point = 0;
        private readonly PropertyInfo[] _props;
        
        public CsvObjectMetadataCursorMover()
        {
            _props = typeof(T).GetProperties();
        }

        public MoveResult MoveNext()
        {
            if (_point >= _props.Length)
            {
                return MoveResult.EndResult();
            }

            var buffer = new StringBuilder();

            buffer.Append(GetAttributeValue(_props[_point]));

            if (_point < _props.Length - 1)
            {
                buffer.Append(',');
            }

            _point++;
            
            return MoveResult.WithResult(buffer.ToString());
        }

        private string GetAttributeValue(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<CsvPropertyNameAttribute>();

            if (attr == null)
            {
                return prop.Name;
            }

            return attr.Name;
        }
    }
}