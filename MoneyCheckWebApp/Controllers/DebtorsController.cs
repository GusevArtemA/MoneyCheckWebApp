using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debts;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/debtors/")]
    public class DebtorsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public DebtorsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавлет нового должника контекстному пользователю
        /// </summary>
        /// <param name="debtorType">Объект, репрезентирующий должника</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostDebtor(
            [FromBody]
            DebtorType debtorType)
        {
            var debtor = new Debtor()
            {
                Name = debtorType.Name
            };

            _context.Debtors.Add(debtor);

            debtor.Owner = this.ExtractUser();
            
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Убирает должника у контекстного пользователя
        /// </summary>
        /// <param name="id">Индефикатор</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("remove")]
        public async Task<IActionResult> DeleteDebtor(long id)
        {
            if (await _context.Debtors.AllAsync(x => x.Id != id))
            {
                return BadRequest("Can't find debtor");
            }

            _context.Debts.RemoveRange(_context.Debts.Where(x => x.DebtorId == id));
            
            _context.Debtors.Remove(new Debtor
            {
                Id = id
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Получает должника контекстного пользователя
        /// </summary>
        /// <param name="id">Индефикатор должника</param>
        /// <returns></returns>
        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> GetDebtor(long id)
        {
            if (await _context.Debtors.AllAsync(x => x.Id != id))
            {
                return BadRequest("Can't find debtor");
            }

            var debtor = await _context.Debtors.FirstAsync(x => x.Id == id);

            var connectedDebts = _context.Debts
                .Where(x => x.DebtorId == debtor.Id)
                .Select(x => new DebtType
                {
                    Amount = x.Amount,
                    Description = x.Description,
                    PurchaseId = x.PurchaseId
                });
            
            return Ok(new DebtorCollectorType
            {
                Name = debtor.Name,
                Debts = connectedDebts.ToArray()
            });
        }

        /// <summary>
        /// Меняет имя должника
        /// </summary>
        /// <param name="name">Новое имя должника</param>
        /// <param name="id">Индефикатор должника</param>
        /// <returns></returns>
        [HttpPatch]
        [Route("change-name")]
        public async Task<IActionResult> EditDebtorName(string name, long id)
        {
            if (await _context.Debtors.AllAsync(x => x.Id != id))
            {
                return BadRequest("Can't find debtor");
            }
            
            var debtor = await _context.Debtors.FirstAsync(x => x.Id == id);

            if (debtor.Name == name)
            {
                return BadRequest("New name should be differ from current name");
            }
            
            debtor.Name = name;

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}