using System;
using System.Reflection;
using System.Text;

namespace MoneyCheckWebApp.ExportingServices.Csv.Movers
{
    public class CsvObjectPropertiesCursorMover : ICursorMover
    {
        private readonly object _obj;
        private PropertyInfo[] _props;
        private int _point = 0;

        public CsvObjectPropertiesCursorMover(object obj)
        {
            _obj = obj;
            _props = _obj.GetType().GetProperties();

            if (_props.Length == 0)
            {
                throw new ArgumentException($"There is no public props in {obj.GetType()}");
            }
        }

        public MoveResult MoveNext()
        {
            if (_point >= _props.Length)
            {
                return MoveResult.EndResult();
            }

            var buffer = new StringBuilder();

            buffer.Append(GetPropValueInString(_props[_point]));

            if (_point < _props.Length - 1)
            {
                buffer.Append(',');
            }

            _point++;
            
            return MoveResult.WithResult(buffer.ToString());
        }

        private string GetPropValueInString(PropertyInfo info) => info.GetValue(_obj)!.ToString() ?? String.Empty;
    }
}