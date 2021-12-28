using System.Threading.Tasks;
using MoneyCheckWebApp.Models;

namespace MoneyCheckWebApp.Extensions
{
    public static class CategoriesExtensions
    {
        /// <summary>
        /// Получает наиболее старшую категорию из контекстного графа категорий
        /// </summary>
        public static Category GetTopParentCategory(this Category category)
        {
            Category context = category;

            while (context.ParentCategory != null)
            {
                context = context.ParentCategory;
            }

            return context;
        }

        /// <summary>
        /// Получает наиболее старшую категорию из контекстного графа категорий асинхронно
        /// </summary>
        public static Task<Category> GetTopParentCategoryAsync(this Category category) =>
            Task.FromResult(category.GetTopParentCategory());
    }
}