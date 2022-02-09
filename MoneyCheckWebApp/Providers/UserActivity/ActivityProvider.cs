using System;
using System.Collections.Generic;
using System.Linq;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Providers.UserActivity
{
    public static class ActivityProvider
    {
        public static IEnumerable<Activity> ParseActivity(User user, SearchSpan span)
        {
            var list = new List<Activity>();
            
            var purchases = ParsePurchases(user, span);

            list.AddRange(purchases);

            return list.OrderByDescending(x => x.TimeStamp);
        }

        private static IEnumerable<Activity> ParsePurchases(User user, SearchSpan searchSpan)
        {
            var now = DateTime.Now;
            
            var filteredPurchases = searchSpan switch
            {
                SearchSpan.ByDay => user.Purchases.Where(x => x.BoughtAt.Year == now.Year && x.BoughtAt.Day == now.Day),
                SearchSpan.ByMonth => user.Purchases.Where(x => x.BoughtAt.Year == now.Year && x.BoughtAt.Month == now.Month),
                SearchSpan.ByYear => user.Purchases.Where(x => x.BoughtAt.Year == now.Year),
                _ => throw new ArgumentOutOfRangeException(nameof(searchSpan), searchSpan, null)
            };

            return filteredPurchases.Select(x => new Activity()
            {
                ActivityType = ActivityType.Purchase,
                IconUrl = "/api/media/" + (x.VerifiedCompany == null
                    ? "get-category-media-logo?categoryId=" + x.Category.Id
                    : "get-verified-company-logo?id=" + x.VerifiedCompany!.Id),
                Description = x.VerifiedCompany?.CompanyName ?? x.Category.CategoryName,
                TimeStamp = x.BoughtAt,
                Amount = x.Amount,
                Id = x.Id
            });
        }
    }

    public enum SearchSpan
    {
        ByDay,
        ByMonth,
        ByYear
    }
}