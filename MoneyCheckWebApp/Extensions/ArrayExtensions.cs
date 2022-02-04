using System.Linq;

namespace MoneyCheckWebApp.Extensions
{
    public static class ArrayExtensions
    {
        public static T[] GetLastElements<T>(this T[] array, int last)
        {
            var list = array.ToList();
            
            list.RemoveRange(last, array.Length - last);

            return list.ToArray();
        }
    }
}