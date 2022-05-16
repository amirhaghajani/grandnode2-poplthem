using Microsoft.AspNetCore.Mvc;
using Grand.Web.Controllers;
using Grand.Business.Catalog.Interfaces.Categories;

namespace Grand.Web.Controllers
{
    public class MyCatalogController : BasePublicController
    {
        private readonly ICategoryService _categoryService;

        public MyCatalogController(ICategoryService categoryService)
        {
            this._categoryService = categoryService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> MyCategory()
        {
            var category = await _categoryService.GetCategoryById("shop all");
            return Content("Yes ------------------");
        }
    }
}
