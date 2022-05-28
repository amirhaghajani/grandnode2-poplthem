using Grand.Business.Catalog.Interfaces.Brands;
using Grand.Business.Catalog.Interfaces.Categories;
using Grand.Business.Catalog.Interfaces.Collections;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Checkout.Queries.Models.Orders;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Logging;
using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Services.Security;
using Grand.Business.Customers.Events;
using Grand.Business.Customers.Interfaces;
using Grand.Business.Marketing.Interfaces.Customers;
using Grand.Domain.Catalog;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Vendors;
using Grand.Infrastructure;
using Grand.Web.Commands.Models.Vendors;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Captcha;
using Grand.Web.Features.Models.Catalog;
using Grand.Web.Features.Models.Vendors;
using Grand.Web.Models.Catalog;
using Grand.Web.Models.Vendors;
using Grand.Web.Themes.Popl.Features.Models.Catalog;
using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Grand.Web.Controllers
{
    public class MyCatalogController : BasePublicController
    {
        #region Fields

        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ICollectionService _collectionService;
        private readonly IWorkContext _workContext;
        private readonly IGroupService _groupService;
        private readonly ITranslationService _translationService;
        private readonly IUserFieldService _userFieldService;
        private readonly IAclService _aclService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerActionEventService _customerActionEventService;
        private readonly IMediator _mediator;

        private readonly VendorSettings _vendorSettings;

        #endregion

        public MyCatalogController(IVendorService vendorService,
            ICategoryService categoryService,
            IBrandService brandService,
            ICollectionService collectionService,
            IWorkContext workContext,
            IGroupService groupService,
            ITranslationService translationService,
            IUserFieldService userFieldService,
            IAclService aclService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ICustomerActionEventService customerActionEventService,
            IMediator mediator,
            VendorSettings vendorSettings)
        {
            _vendorService = vendorService;
            _categoryService = categoryService;
            _brandService = brandService;
            _collectionService = collectionService;
            _workContext = workContext;
            _groupService = groupService;
            _translationService = translationService;
            _userFieldService = userFieldService;
            _aclService = aclService;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _customerActionEventService = customerActionEventService;
            _mediator = mediator;
            _vendorSettings = vendorSettings;
        }


        public IActionResult Index()
        {
            return View();
        }

        #region Utilities

        protected async Task SaveLastContinueShoppingPage(Customer customer)
        {
            await _userFieldService.SaveField(customer,
                SystemCustomerFieldNames.LastContinueShoppingPage,
                HttpContext?.Request?.GetDisplayUrl(),
                _workContext.CurrentStore.Id);
        }

        private VendorReviewOverviewModel PrepareVendorReviewOverviewModel(Vendor vendor)
        {
            var model = new VendorReviewOverviewModel() {
                RatingSum = vendor.ApprovedRatingSum,
                TotalReviews = vendor.ApprovedTotalReviews,
                VendorId = vendor.Id,
                AllowCustomerReviews = vendor.AllowCustomerReviews
            };
            return model;
        }

        #endregion

        public virtual async Task<IActionResult> MyCategoryShopAll(string categoryId, CatalogPagingFilteringModel command)
        {
            var category = await _categoryService.GetCategoryById(categoryId);
            if (category == null)
                return InvokeHttp404();

            var customer = _workContext.CurrentCustomer;

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !await _permissionService.Authorize(StandardPermission.ManageCategories, customer))
                return InvokeHttp404();

            //ACL (access control list)
            if (!_aclService.Authorize(category, customer))
                return InvokeHttp404();

            //Store access
            if (!_aclService.Authorize(category, _workContext.CurrentStore.Id))
                return InvokeHttp404();

            //'Continue shopping' URL
            await SaveLastContinueShoppingPage(customer);

            //display "edit" (manage) link
            if (await _permissionService.Authorize(StandardPermission.AccessAdminPanel, customer) && await _permissionService.Authorize(StandardPermission.ManageCategories, customer))
                DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = "Admin" }));

            //activity log
            _ = _customerActivityService.InsertActivity("PublicStore.ViewCategory", category.Id,
                _workContext.CurrentCustomer, HttpContext.Connection?.RemoteIpAddress?.ToString(),
                _translationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);
            await _customerActionEventService.Viewed(customer, HttpContext.Request.Path.ToString(), Request.Headers[HeaderNames.Referer].ToString() != null ? Request.Headers["Referer"].ToString() : "");


            //aha.com -------------------------------------------------
            //model
            var model = await _mediator.Send(new MyGetAllCategories() {
                Category = category,
                Command = command,
                Currency = _workContext.WorkingCurrency,
                Customer = _workContext.CurrentCustomer,
                Language = _workContext.WorkingLanguage,
                Store = _workContext.CurrentStore
            });

            //layout
            var layoutViewPath = await _mediator.Send(new GetCategoryLayoutViewPath() { LayoutId = category.CategoryLayoutId });

            return View(layoutViewPath, model);
        }
    }
}
