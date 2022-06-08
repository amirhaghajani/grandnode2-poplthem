using Microsoft.AspNetCore.Mvc;
using Grand.Web.Common.Components;

namespace Grand.Web.Themes.Popl.Components
{
    public class MyPoplFeatures : BaseViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            IViewComponentResult answer = View();
            return Task.FromResult(answer);
        }
    }
}
