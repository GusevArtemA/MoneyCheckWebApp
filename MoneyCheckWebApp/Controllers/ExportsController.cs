using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.ExportingServices.Csv;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Purchases;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/exports")]
    public class ExportsController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public ExportsController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("csv-purchases")]
        public IActionResult GetCsvPurchases()
        {
            var invoker = this.ExtractUser();

            var purchasesType = invoker.Purchases.Where(x => x.BoughtAt.Year == DateTime.Today.Year).Select(x =>
                new PurchaseType()
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BoughtAt = x.BoughtAt,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.CategoryName,
                    Latitude = x.Latitude ?? 0,
                    Longitude = x.Longitude ?? 0
                });

            var csvConverter = new CsvConverter<PurchaseType>();
            
            var csv = csvConverter.ConvertEnumerable(purchasesType);

            var csvBytes = Encoding.UTF8.GetBytes(csv);

            return File(csvBytes,
                "application/csv",
            $"Транзакции пользователя {invoker.Username} за {DateTime.Today.Year} год.csv");
        }
    }
}