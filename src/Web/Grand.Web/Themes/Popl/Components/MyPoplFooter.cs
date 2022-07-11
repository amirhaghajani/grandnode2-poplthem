using Grand.Web.Common.Components;
using Grand.Web.Features.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.Themes.Popl.Components
{
    public class MyPoplFooter : BaseViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            IViewComponentResult answer = View();
            return Task.FromResult(answer);
        }
    }
}
