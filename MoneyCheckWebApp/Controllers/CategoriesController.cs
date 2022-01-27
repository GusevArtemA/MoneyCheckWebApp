using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;
using MoneyCheckWebApp.Types.Purchases;

namespace MoneyCheckWebApp.Controllers
{
    [ApiController]
    [Route("/api/categories/")]
    public class CategoriesController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public CategoriesController(MoneyCheckDbContext context)
        {
            _context = context;
        }
        
        /// <summary>
        /// Добавляет новую категорию
        /// </summary>
        /// <param name="category">Объект репрезентирующий категорию</param>
        /// <returns></returns>
        [HttpPost]
        [Route("add-category")]
        public async Task<IActionResult> AddCategoryAsync([FromBody] CategoryType category)
        {
            var invoker = this.ExtractUser();
            
            if (category.ParentCategoryId != null &&
                _context.Categories.FirstOrDefault(x =>
                    x.Id == category.ParentCategoryId) == null)
            {   
                return BadRequest(Statuses.CategoryNotFound);
            }

            if (_context.DefaultLogosForCategories.All(x => x.Id != category.LogoId))
            {
                return BadRequest(Statuses.CategoryLogoRequiredStatus);
            }

            if (_context.Categories.Any(x =>
                    (x.OwnerId == null || x.OwnerId == invoker.Id) && x.CategoryName == category.CategoryName))
            {
                return BadRequest(); //Category already exists
            }
            
            Category addCategory = new()
            {
                CategoryName = category.CategoryName,
                ParentCategoryId = category.ParentCategoryId
            };

            await _context.Categories.AddAsync(addCategory);

            addCategory.Owner = invoker;
            addCategory.Logo = _context.DefaultLogosForCategories.First(x => x.Id == category.LogoId);
            
            await _context.SaveChangesAsync();

            return Ok(addCategory.Id);
        }

        [HttpDelete]
        [Route("delete-category")]
        public async Task<IActionResult> DeleteCategory(long id)
        {
            if (await _context.Categories.AnyAsync(x => x.OwnerId == null && x.Id == id) ||
                await _context.Categories.AllAsync(x => x.Id != id))
            {
                return BadRequest();//Delete system category
            }

            _context.Categories.Remove(new Category() {Id = id});

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}