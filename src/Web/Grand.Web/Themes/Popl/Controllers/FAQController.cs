using Grand.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Themes.Popl.Controllers
{
    public class FAQController : BasePublicController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
