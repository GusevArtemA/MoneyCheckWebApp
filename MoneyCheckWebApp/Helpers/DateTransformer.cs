using System;

namespace MoneyCheckWebApp.Helpers
{
    public static class DateTransformer
    {
        public static string ToPrettyString(this DateTime dateTime)
        {
            var now = DateTime.Now;
            var delta = now - dateTime;
            var days = delta.Days;
            var hours = delta.Hours;
            var minutes = delta.Minutes;
            var seconds = delta.Seconds;

            if (seconds is >= 0 and <= 3 &&
                hours == 0 &&
                minutes == 0 &&
                days == 0)
            {
                return "Только что";
            }

            switch (days)
            {
                case 0 when hours == 0 && minutes == 0 && seconds > 0:
                {
                    var firstSecond = seconds % 10;
                    return seconds + $@" {(firstSecond == 1 ? "секунду" : 
                        firstSecond is >= 2 and <= 4 ? "секунды" : "секунд")} назад";
                }
                case 0 when hours == 0 && minutes > 0:
                {
                    var firstMinute = minutes % 10;

                    return minutes + $@" {(firstMinute == 1 ? "минуту" : 
                        firstMinute is >= 2 and <= 4 ? "минуты" : "минут")} назад";
                }
                case 0 when hours > 0:
                {
                    var firstHour = hours % 10;

                    return hours + $@" {(firstHour == 1 ? "час" : 
                        firstHour is >= 2 and <= 4 ? "часа" : "часов")} назад";
                }
                case > 0:
                {
                    var firstDay = days % 10;

                    return days + $@" {(firstDay == 1 ? "день" :
                        firstDay is >= 2 and <= 4 ? "дня" : "дней")} назад";
                }
                default:
                    return "Ошибка";
            }
        }
    }
}