using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.PricePredicating;
using MoneyCheckWebApp.PricePredicating.Exceptions;
using MoneyCheckWebApp.PricePredicating.Extensions;
using MoneyCheckWebApp.Providers.UserActivity;
using MoneyCheckWebApp.Types.UserStats;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/user-stats/")]
    public class UserStatsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public UserStatsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get-future-cash")]
        public IActionResult GetFutureCashSpending()
        {
            try
            {
                var predicator = new PredicationProcessor(this.ExtractUser().ConfigureArrayProvider());

                return Ok(predicator.PredicateNext());
            }
            catch (NoInfoException)
            {
                return Ok(-1);
            }
        }

        [HttpGet]
        [Route("get-trace")]
        public IActionResult GetTraceStats(int? index, string? filter = "year")
        {
            var invoker = this.ExtractUser();
            try
            {
                return Ok(filter switch
                {
                    "year" => invoker.Purchases.Where(x => x.BoughtAt.Year == (index ?? DateTime.Today.Year))
                        .GroupBy(x => x.BoughtAt.Month)
                        .Select(x => new Tuple<int, decimal?>(x.Key,
                            x.Select(z => z.Amount).Sum()))
                        .OrderBy(x => x.Item1)
                        .Select(x => new StatTrace
                        {
                            Index = x.Item1 switch
                            {
                                1 => "Янв",
                                2 => "Фев",
                                3 => "Март",
                                4 => "Апр", 
                                5 => "Май",
                                6 => "Июнь",
                                7 => "Июль",
                                8 => "Авг",
                                9 => "Сен",
                                10 => "Окт",
                                11 => "Нояб",
                                12 => "Дек",
                                _ => "?"
                            },
                            Amount = x.Item2
                        }),
                    "month" => invoker.Purchases.Where(x => x.BoughtAt.Year == DateTime.Now.Year &&
                                                            x.BoughtAt.Month == (index ?? DateTime.Today.Month))
                        .GroupBy(x => x.BoughtAt.Month)
                        .Select(x => new Tuple<int, IEnumerable<StatTrace>>(x.Key, x.Select(z => new StatTrace()
                        {
                            Index = z.BoughtAt.Day.ToString(),
                            Amount = z.Amount
                        })))
                        .First(x => x.Item1 == index).Item2
                        .GroupBy(x => x.Index)
                        .Select(x => new StatTrace()
                        {
                            Index = x.Key,
                            Amount = x.Select(z => z.Amount).Sum()
                        }).OrderBy(x => int.Parse(x.Index)),
                    "years" => invoker.Purchases.GroupBy(x => x.BoughtAt.Year)
                        .OrderBy(x => x.Key)
                        .Select(x => new StatTrace()
                        {
                            Index = x.Key.ToString(),
                            Amount = x.Select(z => z.Amount).Sum()
                        }),
                    _ => Array.Empty<StatTrace>()
                });
            }
            catch (Exception)
            {
                return BadRequest("Failed get trace");
            }
        }

        [HttpGet]
        [Route("get-transactions")]
        public IActionResult GetTransactions(string filter)
        {
            var invoker = this.ExtractUser();
            var searchLimit = filter switch
            {
                "today" => TimeSpan.FromDays(1),
                "month" => DateTime.Today - DateTime.Today.AddMonths(-1),
                "year" => TimeSpan.FromDays(365),
                _ => TimeSpan.FromDays(1)
            };

            var parsed = ActivityProvider.ParseActivity(invoker, searchLimit);

            return Ok(parsed);
        }

        /// <summary>
        /// Получает транзакции сгрппированные по категориям
        /// </summary>
        /// <param name="from">Начало отсчета в UNIX</param>
        /// <param name="to">Конец отсчета в UNIX</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-categories-data")]
        public IActionResult GetCategoriesData(DateTime from, DateTime to)
        {
            if (from >= to)
            {
                return BadRequest("Failed to get categories by this time span");
            }

            var invoker = this.ExtractUser();

            var groupedCategories = invoker.Purchases.Where(x => from <= x.BoughtAt && x.BoughtAt <= to).ToList()
                .GroupBy(x => x.Category).ToList();

            if (groupedCategories == null)
            {
                throw new InvalidOperationException("GroupedCategories are null");
            }

            CategoryDataType ParseCategoryGrouping(Category grouping)
            {
                var contextCategory = grouping;

                var allChildCategories = groupedCategories!.Where(x =>
                        x.Key.ParentCategoryId != null &&
                        x.Key.Id != contextCategory.Id &&
                        contextCategory.Id == x.Key.ParentCategoryId)
                    .Select(x => ParseCategoryGrouping(x.Key));

                CategoryDataType categoryDataType = new()
                {
                    Id = contextCategory.Id,
                    ChildCategories = allChildCategories
                };

                decimal GetSumRecursive(CategoryDataType category)
                {
                    var recursiveSum = category.ChildCategories?.Select(GetSumRecursive).Sum() ?? 0; 
                    var sum =  contextCategory.Purchases.Where(x => from <= x.BoughtAt && x.BoughtAt <= to).Select(x => x.Amount).Sum() + recursiveSum;

                    categoryDataType.CategoryAmount = sum;

                    return sum;
                }

                GetSumRecursive(categoryDataType);

                return categoryDataType;
            } 
            
            IEnumerable<CategoryDataType> categories = _context.Categories.Where(x =>
                    (x.OwnerId == null || x.OwnerId == invoker.Id) && x.ParentCategoryId == null).ToList()
                .Select(ParseCategoryGrouping);

            return Ok(categories);
        }
    }
}