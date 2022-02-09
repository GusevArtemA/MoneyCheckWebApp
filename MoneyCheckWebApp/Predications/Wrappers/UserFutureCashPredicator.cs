using System;
using System.Linq;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Predications.LinearRegression;
using MoneyCheckWebApp.Predications.LinearRegression.Extensions;

namespace MoneyCheckWebApp.Predications.Wrappers
{
    /// <summary>
    /// Обертка над <see cref="LinearRegressionPredicationProcessor"/>, которая помогает абстрагироваться от абстракций, которые использует <see cref="LinearRegressionPredicationProcessor"/>.
    /// Производит расчет трат средств до конца текущего месяца. 
    /// </summary>
    public static class UserFutureCashPredicator
    {
        public static decimal PredicateToEndOfMonth(this User user)
        {
            try
            {
                IDecimalArrayProvider userArrayProvider = user.ConfigureArrayProvider();
                var processor = LinearRegressionPredicationProcessor.Initialize(userArrayProvider);
                var now = DateTime.Now;
               
                return processor.PredicateNext(DateTime.DaysInMonth(now.Year, now.Month));
            }
            catch (Exception)
            {
                return -1m;
            }
        }
    }
}