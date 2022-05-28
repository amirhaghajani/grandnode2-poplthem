using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Services.Security;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Grand.Web.Features.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.ViewComponents
{
    public class MyHomeSliderViewComponent : BaseViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync()
        {
            IViewComponentResult answer = View();
            return Task.FromResult(answer);
        }
    }
}
