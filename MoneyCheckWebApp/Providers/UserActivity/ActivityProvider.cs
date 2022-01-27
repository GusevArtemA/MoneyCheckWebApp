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
            
            var purchases = ParsePurchases(user, maxDate);

            list.AddRange(purchases);

            return list.OrderByDescending(x => x.TimeStamp);
        }

        private static IEnumerable<Activity> ParsePurchases(User user, DateTime maxDate)
        {
            return user.Purchases.FilterByDateTime(x => x.BoughtAt, maxDate.Date).Select(x => new Activity()
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

        private static IEnumerable<T> FilterByDateTime<T>(this IEnumerable<T> array, Func<T, DateTime> dateTime, DateTime predict, bool more = true)
        {
            return array.Where(x =>
            {
                var date = dateTime(x).Date;
                
                var exr = date > predict && date <= DateTime.Today;

                if (!more)
                {
                    exr = !exr;
                }

                return exr;
            });
        }
    }
}