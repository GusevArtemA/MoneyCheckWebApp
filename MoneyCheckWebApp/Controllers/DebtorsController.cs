using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Debtors;
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

        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> PostDebtor(
            [FromBody]
            DebtorType debtorType)
        {
            User? naturalMaskInstance = null; 
            
            if (debtorType.NaturalMaskId != null)
            {
                naturalMaskInstance = await _context.Users.FirstOrDefaultAsync(x => x.Id == debtorType.NaturalMaskId);

                if (naturalMaskInstance == null)
                {
                    return BadRequest("Can't find debtor by mask");
                }
            }

            var debtor = new Debtor()
            {
                Name = debtorType.Name
            };

            _context.Debtors.Add(debtor);

            if (naturalMaskInstance != null)
            {
                debtor.NaturalMask = naturalMaskInstance;    
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        [Route("remove")]
        public async Task<IActionResult> DeleteDebtor(long id)
        {
            if (await _context.Debtors.AllAsync(x => x.Id != id))
            {
                return BadRequest("Can't find debtor");
            }

            _context.Debtors.Remove(new Debtor
            {
                Id = id
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

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
                    Count = x.Count,
                    Description = x.Description,
                    PurchaseId = x.PurchaseId,
                    DebtorId = x.DebtorId
                });
            
            return Ok(new DebtorCollectorType
            {
                Name = debtor.Name,
                NaturalMaskId = debtor.NaturalMaskId,
                Debts = connectedDebts.ToArray()
            });
        }

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