using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;

namespace Payments.IDPayIR.Controllers
{
    [ViewComponent(Name = "PaymentIDPayIR")]
    public class PaymentIDPayIRViewComponent : BaseViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(this.GetViewPath());
        }
    }
}