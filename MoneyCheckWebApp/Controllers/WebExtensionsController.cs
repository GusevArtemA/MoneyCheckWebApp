using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Predications.Exceptions;
using MoneyCheckWebApp.Predications.LinearRegression;
using MoneyCheckWebApp.Predications.LinearRegression.Extensions;
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

        public WebExtensionsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает данные в удобном виде для веб версии, где нужно минимизировать количество запросов
        /// </summary>
        [HttpGet]
        [Route("user-balance-stats")]
        public IActionResult GetUserBalanceStats()
        {
            var invoker = this.ExtractUser();
            var predicationProcessor = new PredicationProcessor(invoker.ConfigureArrayProvider());

            var todaySpent = invoker.Purchases.Where(x => 
                    x.BoughtAt.Date == DateTime.Today && x.Category.CategoryName != "Зачисление")
                .Select(x => x.Amount)
                .Sum();

            var futureSpend = 0m;

            try
            {
                futureSpend = predicationProcessor.PredicateNext();
            }
            catch (NoInfoException)
            {
                // ignored
            }

            return Ok(new UserStatsBalanceType()
            {
                Balance = invoker.Balance,
                FutureCash = futureSpend,
                TodaySpent = todaySpent
            });
        }

        [HttpGet]
        [Route("get-categories")]
        public IActionResult GetCategories()
        {
            var invoker = this.ExtractUser();

            List<CategoryType> categories = new List<CategoryType>();

            categories.AddRange(_context.Categories.Where(x => x.OwnerId == null)
                .Select(x => new CategoryType
            {
                Id = x.Id,
                Name = x.CategoryName
            }));

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
    }
}