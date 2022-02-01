using System.Text;

namespace MoneyCheckWebApp.ExportingServices.Csv.Movers
{
    public class CursorService
    {
        private readonly ICursorMover _cursor;

        public CursorService(ICursorMover cursor)
        {
            _cursor = cursor;
        }

        public string ReadToEnd()
        {
            StringBuilder buffer = new();

            for (;;)
            {
                var chunk = _cursor.MoveNext();

                if (chunk.End)
                {
                    break;
                }

                buffer.Append(chunk.Result);
            }
            

            return buffer.ToString();
        }
    }
}