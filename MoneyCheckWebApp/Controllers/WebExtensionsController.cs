using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Predications.InflationPredicating;
using MoneyCheckWebApp.Predications.Wrappers;
using MoneyCheckWebApp.Types.Debts;
using MoneyCheckWebApp.Types.WebExtensions;

namespace MoneyCheckWebApp.Controllers
{
    /// <summary>
    /// Данный контроллер предназначен только для веб версии приложения - он содержит методы, которые обеспечивают минимальное количество запросов, при этом они не предлагают той же функциональности как методы основного API.
    /// </summary>
    [ApiController]
    [Route("/api/web")]
    public class WebExtensionsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;
        private readonly InflationPredicationProcessor _inflationPredicationProcessor;

        public WebExtensionsController(MoneyCheckDbContext context,
            InflationPredicationProcessor inflationPredicationProcessor)
        {
            _context = context;
            _inflationPredicationProcessor = inflationPredicationProcessor;
        }

        /// <summary>
        /// Получает данные в удобном виде для веб версии, где нужно минимизировать количество запросов
        /// </summary>
        [HttpGet]
        [Route("user-balance-stats")]
        public async Task<IActionResult> GetUserBalanceStats()
        {
            var invoker = this.ExtractUser();

            var todaySpent = invoker.Purchases.Where(x => 
                    x.BoughtAt.Date == DateTime.Today && x.Category.CategoryName != "Зачисление")
                .Select(x => x.Amount)
                .Sum();

            var futureSpend = /*invoker.PredicateToEndOfMonth();*/0;
            var inflationRate = await _inflationPredicationProcessor.PredicateAsync(1);
            var inlfationCost = Math.Round((double)invoker.Balance - (double)invoker.Balance * (inflationRate - 1));

            return Ok(new UserStatsBalanceType()
            {
                Balance = invoker.Balance,
                FutureCash = futureSpend,
                TodaySpent = todaySpent,
                InflationCash = inlfationCost
            });
        }

        [HttpGet]
        [Route("get-categories")]
        public IActionResult GetCategories(bool includeDefaultCategories = true)
        {
            var invoker = this.ExtractUser();

            List<CategoryType> categories = new();

            if (includeDefaultCategories)
            {
                categories.AddRange(_context.Categories.Where(x => x.OwnerId == null)
                    .Select(x => new CategoryType
                    {
                        Id = x.Id,
                        Name = x.CategoryName
                    }));    
            }

            categories.AddRange(invoker.Categories.Where(x => x.OwnerId != null && x.OwnerId == invoker.Id)
                .Select(x => new CategoryType()
            {
                Id = x.Id,
                Name = x.CategoryName
            }));

            return Ok(categories);
        }

        [HttpGet]
        [Route("get-debtors")]
        public IActionResult GetDebtors()
        {
            var invoker = this.ExtractUser();

            return Ok(invoker.Debtors.Select(x => new DebtorType
            {
                Id = x.Id,
                Name = x.Name,
                Debts = x.Debts.Select(m => new DebtType
                {
                    Amount = m.Amount,
                    DebtId = m.DebtId,
                    Description = m.Description,
                    PurchaseId = m.PurchaseId
                })
            }));
        }

        [HttpGet]
        [Route("get-inflation-for-year")]
        public async Task<IActionResult> GetInflationForYear()
        {
            InflationChunkType[] chunks = new InflationChunkType[12];
            
            for (int i = 0; i < 12; i++)
            {
                var predication = await _inflationPredicationProcessor.PredicateAsync(i + 1);
                var monthIndex = i + 1;
                chunks[i] = new InflationChunkType()
                {
                    Index =
                        $"{monthIndex}{monthIndex switch {1 or 4 or 5 or 9 or 10 or 11 or 12 => "-ый", 2 or 6 or 7 or 8 => "-ой", _ => "-ий"}} месяц",
                    Value = (predication - 1) * 100
                };
            }

            return Ok(chunks);
        }
        
        [HttpGet]
        [Route("get-def-logos")]
        public IActionResult GetAllDefaultLogos() => Ok(_context.DefaultLogosForCategories.Select(x => new CategoryLogo()
        {
            Id = x.Id,
            Url = "/api/media/get-logo?id=" + x.Id
        }));
    }
}