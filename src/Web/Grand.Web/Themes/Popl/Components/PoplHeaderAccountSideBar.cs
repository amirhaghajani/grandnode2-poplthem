using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Services.Security;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Grand.Web.Features.Models.Catalog;
using Grand.Web.Features.Models.Customers;
using Grand.Web.Models.Customer;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Grand.Web.ViewComponents
{
    public class PoplHeaderAccountSideBarViewComponent : BaseViewComponent
    {
        private readonly IMediator _mediator;
        private readonly IWorkContext _workContext;
        private readonly IPermissionService _permissionService;
        private readonly IGroupService _groupService;

        public PoplHeaderAccountSideBarViewComponent(
            IMediator mediator,
            IWorkContext workContext,
            IPermissionService permissionService,
            IGroupService groupService)
        {
            _mediator = mediator;
            _workContext = workContext;
            _permissionService = permissionService;
            _groupService = groupService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!await _groupService.IsRegistered(_workContext.CurrentCustomer))
            {
                var emptyModel =  new CustomerInfoModel();

                return View(emptyModel);
            }

            CustomerInfoModel model = await _mediator.Send(new GetInfo() {
                Customer = _workContext.CurrentCustomer,
                ExcludeProperties = false,
                Language = _workContext.WorkingLanguage,
                Store = _workContext.CurrentStore,
            });

            return View(model);
        }
    }
}