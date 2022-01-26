using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Purchases;
using MoneyCheckWebApp.Extensions;

namespace MoneyCheckWebApp.Controllers
{
    /// <summary>
    /// Контроллер для управления транзакциями
    /// </summary>
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private MoneyCheckDbContext _context;

        public TransactionController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавляет новую транзакцию пользователю
        /// </summary>
        /// <param name="purchase"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-purchase")]
        public async Task<IActionResult> AddPurchaseAsync([FromBody] PostPurchaseType purchase)
        {
            //TODO Add error statuses
            if ((purchase.BoughtAt - DateTime.Now) > TimeSpan.FromDays(1))
            {
                return BadRequest("Date error");
            }

            if (purchase.CategoryId == default)
            {
                return BadRequest("Category not specified");
            }

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == purchase.CategoryId);
            
            if (category == null)
            {
                return BadRequest("Category not found");
            }

            if (purchase.VerifiedCompanyId != null &&
                _context.VerifiedCompanies.All(x => x.Id != purchase.VerifiedCompanyId))
            {
                return BadRequest("Verified company not found");
            }

            var addPurchase = new Purchase()
            {
                BoughtAt = purchase.BoughtAt ?? DateTime.Now,
                Amount = Math.Abs(purchase.Amount), 
                CategoryId = purchase.CategoryId,
                CustomerId = this.ExtractUser().Id
            };

            if (_context.Categories.FirstOrDefault(x => x.Id == purchase.CategoryId) == null)
            {
                BadRequest("Category error");
            }

            await _context.Purchases.AddAsync(addPurchase);

            if (purchase.VerifiedCompanyId != null)
            {
                addPurchase.VerifiedCompanyId = purchase.VerifiedCompanyId;    
            }
            
            this.ExtractUser().Balance -= purchase.Amount  * (category.CategoryName == Resources.AddCashCategoryName ? -1 : 1); //Проверка на зачисление

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Удаляет транзакцию пользователя
        /// </summary>
        /// <param name="id">Индефикатор транзакции</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("remove-purchase")]
        public async Task<IActionResult> RemovePurchaseAsync(long id)
        {
            var extractedUser = this.ExtractUser();
            var foundPurchase = _context.Purchases.SingleOrDefault(x => x.Id == id && x.CustomerId == extractedUser.Id);
            if (foundPurchase != null)
            {
                extractedUser.Balance += foundPurchase.Amount * (foundPurchase.Category.CategoryName == Resources.AddCashCategoryName ? -1 : 1);
                _context.Purchases.Remove(foundPurchase);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("");
            }

            return Ok();
        }
        
        /// <summary>
        /// Изменяет транзакцию пользователя
        /// </summary>
        /// <param name="purchase">Объект, который репрезентирует транзакцию</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("edit-purchase")]
        public async Task<IActionResult> EditPurchaseAsync([FromBody] PurchaseType purchase)
        {
            var user = this.ExtractUser();
            var beforeEditedPurchase = _context.Purchases.FirstOrDefault(x => x.Id == purchase.Id && x.CustomerId == user.Id);
            
            if (beforeEditedPurchase != null)
            {
                user.Balance += beforeEditedPurchase.Amount;
                user.Balance -= purchase.Amount;

                beforeEditedPurchase.Id = purchase.Id;
                beforeEditedPurchase.Amount = purchase.Amount;
                beforeEditedPurchase.CategoryId = purchase.CategoryId;


                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("");
            }

            return Ok();
        }

        /// <summary>
        /// Получает все транзакции по фильтру
        /// </summary>
        /// <param name="filter">Возможные варианты фильтра: за день (day), за месяц (month), за год (year)</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-purchases")]
        public IActionResult GetPurchases(string filter = "none")
        {
            var userId = this.ExtractUser().Id;
            
            var list = _context.Purchases.Where(x => x.CustomerId == userId)
                .Select(x => new PurchaseType()
                {
                    Amount = x.Amount,
                    BoughtAt = x.BoughtAt,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.CategoryName,
                    Id = x.Id,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude
                });

            if (filter == "none") return Ok(list);
            
            switch (filter)
            {
                case "by_today":
                    list = list.Where(x => x.BoughtAt.Date == DateTime.Today);
                    break;
                case "by_this_month":
                    list = list.Where(x => x.BoughtAt.Month == DateTime.Today.Month);
                    break;
                case "by_this_year":
                    list = list.Where(x => x.BoughtAt.Year == DateTime.Today.Year);
                    break;
                default:
                    return BadRequest(Statuses.UnknownFilterStatus);
            }

            return Ok(list);
        }
    }
}
