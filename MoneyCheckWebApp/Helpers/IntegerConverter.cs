namespace MoneyCheckWebApp.Helpers
{
    public static class IntegerConverter
    {
        public static string ToPrettyString(this int a)
        {
            if (a < 1000)
            {
                return a.ToString();
            }

            var length = a.ToString().Length;

            return length switch
            {
                //Тысячи
                >= 4 and <= 6 => a / 1000 % 10 + " тыс",
                >= 7 and <= 9 => a / 100000 % 10 + " млн",
                >= 10 => a / 1000000000 + " млрд",
                _ => a.ToString()
            };
        }
    }
}