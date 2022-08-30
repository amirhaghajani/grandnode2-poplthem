using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;

namespace Payments.BahamtaIR.Controllers
{
    [ViewComponent(Name = "PaymentBahamtaIR")]
    public class PaymentBahamtaIRViewComponent : BaseViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View(this.GetViewPath());
        }
    }
}