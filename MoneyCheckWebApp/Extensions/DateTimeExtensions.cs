using System;

namespace MoneyCheckWebApp.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ToDateTimeUnix(this double seconds)
        {
            var unix = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            return unix.AddSeconds(seconds);
        }
    }
}