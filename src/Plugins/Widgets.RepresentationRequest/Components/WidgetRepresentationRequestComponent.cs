using Grand.Business.Common.Extensions;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;
using Widgets.RepresentationRequest.Models;
using Widgets.RepresentationRequest.Services;

namespace Widgets.RepresentationRequest.Components
{
    [ViewComponent(Name = "WidgetRepresentationRequest")]
    public class WidgetRepresentationRequestComponent : ViewComponent
    {
        private readonly IRepresentationRequestService _requestService;
        private readonly IWorkContext _workContext;

        public WidgetRepresentationRequestComponent(IWorkContext wc, IRepresentationRequestService service )
        {
            this._requestService = service;
            this._workContext = wc;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            var answers = await this._requestService.GetRequests();
            var model = new RepresentationRequestModel();
            return View(this.GetViewPath(), model);
        }
    }
}
