using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Purchases;
using MoneyCheckWebApp.Extensions;

namespace MoneyCheckWebApp.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private MoneyCheckDbContext _context;

        public TransactionController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add-purchase")]
        public async Task<IActionResult> AddPurchaseAsync([FromBody] PostPurchaseType purchase)
        {
            if ((purchase.BoughtAt - DateTime.Now) > TimeSpan.FromDays(1))
            {
                return BadRequest("Date error");
            }

            if (purchase.CategoryId == default)
            {
                return BadRequest("Category not specified");
            }

            if (_context.Categories.All(x => x.Id != purchase.CategoryId))
            {
                return BadRequest("Category not found");
            }
            
            var addPurchase = new Purchase()
            {
                BoughtAt = purchase.BoughtAt ?? DateTime.Now,
                Amount = purchase.Amount,
                CategoryId = purchase.CategoryId,
                CustomerId = this.ExtractUser().Id
            };

            if (_context.Categories.FirstOrDefault(x => x.Id == purchase.CategoryId) == null)
            {
                BadRequest("Category error");
            }

            await _context.Purchases.AddAsync(addPurchase);

            this.ExtractUser().Balance -= purchase.Amount;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("remove-purchase")]
        public async Task<IActionResult> RemovePurchaseAsync(long id)
        {
            var extractedUser = this.ExtractUser();
            var foundPurchase = _context.Purchases.SingleOrDefault(x => x.Id == id && x.CustomerId == extractedUser.Id);
            if (foundPurchase != null)
            {
                extractedUser.Balance += foundPurchase.Amount;
                _context.Purchases.Remove(foundPurchase);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("");
            }

            return Ok();
        }



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

        [HttpPost]
        [Route("add-category")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryType category)
        {
            if (category.ParentCategoryId != null && _context.Categories.FirstOrDefault(x => x.Id == category.ParentCategoryId) == null)
                return BadRequest();

            Category addCategory = new()
            {
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId
            };

            await _context.Categories.AddAsync(addCategory);

            addCategory.Owner = this.ExtractUser();
            
            await _context.SaveChangesAsync();

            return Ok(addCategory.Id);
        }

        [HttpGet]
        [Route("get-purchases")]
        public IActionResult GetPurchases()
        {
            long userId = this.ExtractUser().Id;
           
            var list = _context.Purchases.Where(x => x.CustomerId == userId)
                .Select(x => new PurchaseType()
                {
                    Amount = x.Amount,
                    BoughtAt = x.BoughtAt,
                    CategoryId = x.CategoryId,
                    Id = x.Id,
                    Longitude = x.Longitude,
                    Latitude = x.Latitude
                });

            return Ok(list);
        }
    }
}
