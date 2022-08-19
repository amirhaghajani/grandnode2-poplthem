using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs.Components
{
    [ViewComponent(Name = "WidgetFAQs")]
    public class WidgetFAQsComponent : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            return Content("");
        }
    }
}
