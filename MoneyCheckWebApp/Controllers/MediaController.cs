using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoneyCheckWebApp.Extensions;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Controllers
{
    /// <summary>
    /// Котроллер, который отвечает за менеджмент медиа-ресурсов
    /// </summary>
    [ApiController]
    [Route("api/media/")]
    public class MediaController : ControllerBase
    {
        private readonly MoneyCheckDbContext _context;

        public MediaController(MoneyCheckDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает SVG картинку дефолтной категории
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="includeDefaultCategories"></param>
        [HttpGet]
        [Route("get-category-media-logo")]
        public async Task GetDefaultCategoryMediaLogo(long categoryId)
        {
            var invoker = this.ExtractUser();
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == categoryId);

            if (category == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync(Statuses.CategoryNotFound);
                return;
            }
            
            if (category.OwnerId != invoker.Id && category.OwnerId != null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync(Statuses.NonAccessToContextCategoryStatus);
                return;
            }
            
            var logo = category.Logo;

            if (logo == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync(Statuses.CategoryLogoFailedStatus);
                return;
            }

            Response.ContentType = "image/svg+xml";

            await Response.WriteAsync(logo.Svg);
        }

        /// <summary>
        /// Получает картинку верифицированной компании
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        [Route("get-verified-company-logo")]
        public async Task GetVerifiedBrandLogo(long id)
        {
            var company = await _context.VerifiedCompanies.FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync(Statuses.CompanyNotFoundStatus);
                return;
            }

            Response.ContentType = "image/svg+xml";
            await Response.WriteAsync(company.LogoSvg);
        }
    }
}