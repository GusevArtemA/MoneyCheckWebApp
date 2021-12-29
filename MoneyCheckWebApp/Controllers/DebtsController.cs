using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyCheckWebApp.Controllers
{
    [Route("api/debts")]
    [ApiController]
    public class DebtsController : ControllerBase
    {
        private MoneyCheckDbContext _context;
        public DebtsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("add-debt")]
        public async Task<IActionResult> AddDebt([FromBody] DebtType debtType)
        {
            Debt debt = new Debt()
            {
                DebtorId = debtType.DebtorId,
                Description = debtType.Description,
                PurchaseId = debtType.PurchaseId,
                Count = debtType.Count,
                InitiatorId = this.ExtractUser().Id
            };


            
            if (_context.Debtors.All(x => x.Id != debt.DebtId))
            {
                return BadRequest("Debtor was not found");
            }
            else if (debt.PurchaseId != null && _context.Purchases.FirstOrDefault(x => x.Id == debt.PurchaseId) == null)
            {
                return BadRequest("Purchase was not found");
            }

            await _context.Debts.AddAsync(debt);

            this.ExtractUser().Balance -= debt.Count;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Route("remove-debt")]
        public async Task<IActionResult> RemoveDebt(long id)
        {
            var extractedUser = this.ExtractUser();
            var foundDebt = _context.Debts.SingleOrDefault(x => x.DebtId == id && x.InitiatorId == extractedUser.Id);
            if (foundDebt != null)
            {
                extractedUser.Balance += foundDebt.Count;
                _context.Debts.Remove(foundDebt);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("");
            }

            return Ok();
        }

        [HttpPost]
        [Route("edit-debt")]
        public async Task<IActionResult> EditDebt([FromBody] DebtType debt)
        {
            var user = this.ExtractUser();
            var beforeEditedDebt = _context.Debts.FirstOrDefault(x => x.DebtId == debt.DebtId && x.InitiatorId == user.Id);

            if (beforeEditedDebt == null) return BadRequest("");

            user.Balance += beforeEditedDebt.Count;
            user.Balance -= debt.Count;

            beforeEditedDebt.Count = debt.Count;
            beforeEditedDebt.Description = debt.Description;
            beforeEditedDebt.PurchaseId = debt.PurchaseId;

            await _context.SaveChangesAsync();


            return Ok();
        }

        [HttpGet]
        [Route("get-debts")]
        public IActionResult GetDebts()
        {
            long userId = this.ExtractUser().Id;

            var list = _context.Debts.Where(x => x.InitiatorId == userId)
                .Select(x => new DebtType()
                {
                    DebtId = x.DebtId,
                    Count = x.Count,
                    PurchaseId = x.PurchaseId,
                    DebtorId = x.DebtorId,
                    Description = x.Description
                });

            return Ok(list);
        }

    }
}
