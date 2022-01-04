using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Purchases;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/categories/")]
    public class CategoriesController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public CategoriesController(MoneyCheckDbContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        [Route("add-category")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryType category)
        {
            if (category.ParentCategoryId != null &&
                _context.Categories.FirstOrDefault(x =>
                    x.Id == category.ParentCategoryId) == null)
            {   
                return BadRequest();
            }

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
    }
}