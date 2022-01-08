using System;
using System.Collections.Generic;
using System.Linq;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Providers.UserActivity
{
    public static class ActivityProvider
    {
        public static IEnumerable<Activity> ParseActivity(User user, TimeSpan searchSpan)
        {
            var maxDate = DateTime.Now - searchSpan;
            var list = new List<Activity>();
            
            var debts = ParseDebts(user, maxDate);
            var purchases = ParsePurchases(user, maxDate);
            var accountUpdates = ParseAccountUpdates(user, maxDate);
            
            list.AddRange(debts);
            list.AddRange(purchases);
            list.AddRange(accountUpdates);

            return list;
        }

        private static IEnumerable<Activity> ParseDebts(User user, DateTime maxDate)
        {
            var arrays = user.Debts.Select(x => x.DebtUpdates)
                .Select(x => x.Where(z => z.UpdateAt > maxDate))
                .Select(x => x.Select(z => new Activity()
                {
                    ActivityType = z.Amount > 0 ?  ActivityType.DebtMinus : ActivityType.DebtAdd,
                    IconUrl = z.Amount < 0 ? "/images/plus.svg" : "/images/minus.svg",
                    Description = (z.Amount < 0 ? "Увеличение" : "Уменьшение") + $" долга должника {z.Debt.Debtor.Name}",
                    TimeStamp = z.UpdateAt,
                    Amount = z.Amount
                }));

            List<Activity> activities = new();
            
            foreach (var enumerable in arrays)
            {
                activities.AddRange(enumerable);
            }

            return activities;
        }

        private static IEnumerable<Activity> ParsePurchases(User user, DateTime maxDate)
        {
            return user.Purchases.Where(x => x.BoughtAt > maxDate).Select(x => new Activity()
            {
                ActivityType = ActivityType.Purchase,
                IconUrl = "/api/media/" + (x.VerifiedCompany == null
                    ? "get-category-media-logo?categoryId=" + x.Category.Id
                    : "get-verified-company-logo?id=" + x.VerifiedCompany!.Id),
                Description = "Покупка товара категории " + x.Category.CategoryName,
                TimeStamp = x.BoughtAt,
                Amount = x.Amount
            });
        }

        private static IEnumerable<Activity> ParseAccountUpdates(User user, DateTime maxDate)
        {
            return user.AccountBalanceUpdates.Where(x => x.UpdateAt > maxDate).Select(x => new Activity()
            {
                ActivityType = x.Amount > 0 ? ActivityType.BalanceAdd : ActivityType.BalanceMinus,
                Description = x.OperationType == 1
                    ? x.Amount > 0
                        ? $"Увеличение баланса"
                        : $"Уменьшение баланса"
                    : $"Установка баланса",
                TimeStamp = x.UpdateAt,
                IconUrl = x.OperationType == 1 ? x.Amount > 0 ? "/images/plus.svg" : "/images/minus.svg" : "/images/money-pig.svg",
                Amount = x.Amount
            });
        }
    }
}