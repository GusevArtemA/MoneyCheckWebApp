using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Predications.LinearRegression.Extensions
{
    public static class PurchasesContextDatabaseArrayProviderExtension
    {
        public static IDecimalArrayProvider ConfigureArrayProvider(this User user) =>
            new PurchasesDecimalArrayProvider(user);
    }
}