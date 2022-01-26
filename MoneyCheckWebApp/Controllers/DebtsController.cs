using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MoneyCheckWebApp.Controllers
{
    [Route("/api/debts/")]
    [ApiController]
    [Produces("application/json")]
    public class DebtsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;
        public DebtsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Записывает новый долг должнику контекстного пользователя
        /// </summary>
        /// <param name="debtType">Объект, который реперзентирует долг</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-debt")]
        public async Task<IActionResult> AddDebt([FromBody] PostDebtType debtType)
        {
            var invoker = this.ExtractUser();
            Debt debt = new()
            {
                DebtorId = debtType.DebtorId,
                Description = debtType.Description,
                PurchaseId = debtType.PurchaseId,
                Amount = debtType.Amount,
                InitiatorId = invoker.Id
            };

            if (!_context.Debtors.Any(x => x.Id == debt.DebtorId))
            {
                return BadRequest("Debtor was not found");
            }

            if (debt.PurchaseId != null &&
                _context.Purchases.FirstOrDefault(x => 
                    x.Id == debt.PurchaseId) == null)
            {
                return BadRequest("Purchase was not found");
            }

            await _context.Debts.AddAsync(debt);

            invoker.Balance -= debt.Amount;

            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Списывает долг у должника контекстного пользователя
        /// </summary>
        /// <param name="id">Индефикатор долга</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("remove-debt")]
        public async Task<IActionResult> RemoveDebt(long id)
        {
            var extractedUser = this.ExtractUser();
            var foundDebt = _context.Debts.SingleOrDefault(x => x.DebtId == id && x.InitiatorId == extractedUser.Id);

            if (foundDebt == null) return BadRequest("Debt was not found");

            extractedUser.Balance += foundDebt.Amount;

            _context.Debts.Remove(foundDebt);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Редактирует долг
        /// </summary>
        /// <param name="debt">Новое значение долга</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("edit-debt")]
        public async Task<IActionResult> EditDebt([FromBody] EditDebtType debt)
        {   
            var user = this.ExtractUser();
            var beforeEditedDebt = _context.Debts.FirstOrDefault(x => x.DebtId == debt.DebtId && x.InitiatorId == user.Id);
            if (beforeEditedDebt == null) return BadRequest("");

            var cachedAmount = beforeEditedDebt.Amount;
            
            user.Balance += beforeEditedDebt.Amount;
            user.Balance -= debt.Amount;
                
            beforeEditedDebt.Amount = debt.Amount;
            beforeEditedDebt.Description = debt.Description;
            beforeEditedDebt.PurchaseId = debt.PurchaseId;

            await _context.SaveChangesAsync();

            if (cachedAmount == beforeEditedDebt.Amount) return Ok();
            
            var delta = cachedAmount - beforeEditedDebt.Amount;

            await _context.SaveChangesAsync();
            
            return Ok();
        }

        /// <summary>
        /// Получает все долги должника
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("get-debts")]
        public IActionResult GetDebts()
        {
            long userId = this.ExtractUser().Id;

            var list = _context.Debts.Where(x => x.InitiatorId == userId)
                .Select(x => new DebtType()
                {
                    DebtId = x.DebtId,
                    Amount = x.Amount,
                    PurchaseId = x.PurchaseId,
                    Debtor = _context.Debtors.FirstOrDefault(z => z.Id == x.DebtorId)!.GenerateApiType(),
                    Description = x.Description
                });

            return Ok(list);
        }
    }
}
