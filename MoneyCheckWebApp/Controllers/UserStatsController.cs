using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.PricePredicating;
using MoneyCheckWebApp.PricePredicating.Exceptions;
using MoneyCheckWebApp.PricePredicating.Extensions;
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
        [Route("get-for-year")]
        public IActionResult GetYearStats()
        {
            var invoker = this.ExtractUser();
            var now = DateTime.Now;
            
            return Ok(new StatsForYear()
            {
                Months = invoker.Purchases.Where(x => x.BoughtAt.Year != now.Year)
                    .GroupBy(x => x.BoughtAt.Month)
                    .Select(x => new Tuple<int, Purchase?>(x.Key,
                        x.FirstOrDefault()))
                    .Select(x => new StatForMonth
                    {
                        Number = x.Item1,
                        Amount = x.Item2?.Amount ?? -1
                    }) 
            });
        }
    }
}