using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Helpers;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Stats;

namespace MoneyCheckWebApp.Controllers
{
    [Route("/stats/")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public StatsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("get")]
        public IActionResult GetStats()
        {
            var allTransactionsToday = _context.Purchases.Where(x => x.BoughtAt.Date == DateTime.Today)
                .Select(x => x.BoughtAt).OrderByDescending(x => x);
            var lastTransaction = allTransactionsToday.FirstOrDefault();
            var usersCount = _context.Users.Count();

            return Ok(new StatType
            {
                LastTransaction = !allTransactionsToday.Any() ? "Не было" : lastTransaction.ToPrettyString(),
                PurchasesCountToday = allTransactionsToday.Count().ToPrettyString(),
                PeopleWithUs = usersCount.ToPrettyString()
            });
        }
    }
}