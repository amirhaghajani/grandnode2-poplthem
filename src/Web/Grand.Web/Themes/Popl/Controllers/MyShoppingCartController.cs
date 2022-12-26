using Grand.Business.Catalog.Interfaces.Discounts;
using Grand.Business.Checkout.Extensions;
using Grand.Business.Checkout.Interfaces.CheckoutAttributes;
using Grand.Business.Checkout.Interfaces.GiftVouchers;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Queries.Models.Orders;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Services.Security;
using Grand.Business.Customers.Interfaces;
using Grand.Business.Storage.Extensions;
using Grand.Business.Storage.Interfaces;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Media;
using Grand.Domain.Orders;
using Grand.Infrastructure;
using Grand.Web.Commands.Models.ShoppingCart;
using Grand.Web.Controllers;
using Grand.Web.Features.Models.ShoppingCart;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Grand.Web.Themes.Popl.Controllers
{
    public class MyShoppingCartController : BasePublicController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ITranslationService _translationService;
        private readonly IDiscountService _discountService;
        private readonly ICustomerService _customerService;
        private readonly IGroupService _groupService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IPermissionService _permissionService;
        private readonly IUserFieldService _userFieldService;
        private readonly IMediator _mediator;
        private readonly IShoppingCartValidator _shoppingCartValidator;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly OrderSettings _orderSettings;

        #endregion

        #region Constructors

        public MyShoppingCartController(
            IWorkContext workContext,
            IShoppingCartService shoppingCartService,
            ITranslationService translationService,
            IDiscountService discountService,
            ICustomerService customerService,
            IGroupService groupService,
            ICheckoutAttributeService checkoutAttributeService,
            IPermissionService permissionService,
            IUserFieldService userFieldService,
            IMediator mediator,
            IShoppingCartValidator shoppingCartValidator,
            ShoppingCartSettings shoppingCartSettings,
            OrderSettings orderSettings)
        {
            _workContext = workContext;
            _shoppingCartService = shoppingCartService;
            _translationService = translationService;
            _discountService = discountService;
            _customerService = customerService;
            _groupService = groupService;
            _checkoutAttributeService = checkoutAttributeService;
            _permissionService = permissionService;
            _userFieldService = userFieldService;
            _mediator = mediator;
            _shoppingCartValidator = shoppingCartValidator;
            _shoppingCartSettings = shoppingCartSettings;
            _orderSettings = orderSettings;
        }

        #endregion

        public virtual IActionResult MyContinueShopping()
        {
            var returnUrl = _workContext.CurrentCustomer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.LastContinueShoppingPage, _workContext.CurrentStore.Id);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToRoute("HomePage");

            var host = Request.Scheme + "://" + Request.Host + "/";
            var path = returnUrl.Replace(host, "");

            returnUrl = host + String.Join(
                    "/",
                    path.Split("/").Select(s => System.Net.WebUtility.UrlEncode(s))
                );

            return Redirect(returnUrl);
        }
    }
}
