using Grand.Business.Catalog.Extensions;
using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Services.Orders;
using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Logging;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Orders;
using Grand.Infrastructure;
using Grand.Web.Extensions;
using Grand.Web.Features.Models.Products;
using Grand.Web.Features.Models.ShoppingCart;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

namespace Grand.Web.Controllers
{
    public class MyActionCartController : BasePublicController
    {
        #region Fields

        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IGroupService _groupService;
        private readonly ITranslationService _translationService;
        private readonly ICurrencyService _currencyService;
        private readonly IShoppingCartValidator _shoppingCartValidator;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IMediator _mediator;

        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region CTOR

        public MyActionCartController(IProductService productService,
            IShoppingCartService shoppingCartService,
            IWorkContext workContext,
            IGroupService groupService,
            ITranslationService translationService,
            ICurrencyService currencyService,
            IShoppingCartValidator shoppingCartValidator,
            ICustomerActivityService customerActivityService,
            IMediator mediator,
            ShoppingCartSettings shoppingCartSettings)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _groupService = groupService;
            _translationService = translationService;
            _currencyService = currencyService;
            _shoppingCartValidator = shoppingCartValidator;
            _customerActivityService = customerActivityService;
            _mediator = mediator;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion



        [HttpPost]
        public virtual async Task<IActionResult> AddProductCatalog(string productId, int shoppingCartTypeId,
            int quantity, bool forceredirection = false, IFormCollection form=null)
        {
            var cartType = (ShoppingCartType)shoppingCartTypeId;

            var product = await _productService.GetProductById(productId);
            if (product == null)
                //no product found
                return Json(new
                {
                    success = false,
                    message = "No product found with the specified ID"
                });

            //product and gift voucher attributes
            var attributes = await _mediator.Send(new GetParseProductAttributes() { Product = product, Form = form });

            var redirect = RedirectToProduct(product, cartType, quantity, attributes);
            if (redirect != null)
                return redirect;

            var customer = _workContext.CurrentCustomer;


            

            string warehouseId = GetWarehouse(product);

            var cart = await _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, cartType);

            if (cartType != ShoppingCartType.Wishlist)
            {
                var shoppingCartItem = await _shoppingCartService.FindShoppingCartItem(cart, cartType, product.Id, warehouseId);

                //if we already have the same product in the cart, then use the total quantity to validate
                var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
                var addToCartWarnings = await _shoppingCartValidator
                  .GetShoppingCartItemWarnings(customer, new ShoppingCartItem() {
                      ShoppingCartTypeId = cartType,
                      StoreId = _workContext.CurrentStore.Id,
                      Attributes = attributes,
                      WarehouseId = warehouseId,
                      Quantity = quantityToValidate
                  }, product, new ShoppingCartValidatorOptions() {
                      GetRequiredProductWarnings = false
                  });

                if (addToCartWarnings.Any())
                {
                    //cannot be added to the cart
                    return Json(new
                    {
                        redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                    });
                }
            }

            //try adding product to the cart 
            var addToCart = await _shoppingCartService.AddToCart(customer: customer,
                productId: productId,
                shoppingCartType: cartType,
                storeId: _workContext.CurrentStore.Id,
                warehouseId: warehouseId,
                attributes:attributes,
                quantity: quantity,
                validator: new ShoppingCartValidatorOptions() {
                    GetRequiredProductWarnings = false,
                    GetInventoryWarnings = (cartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist),
                    GetAttributesWarnings = (cartType != ShoppingCartType.Wishlist),
                    GetGiftVoucherWarnings = (cartType != ShoppingCartType.Wishlist)
                });

            if (addToCart.warnings.Any())
            {
                //cannot be added to the cart
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }

            var addtoCartModel = await _mediator.Send(new GetAddToCart() {
                Product = product,
                Attributes =attributes,
                Customer = customer,
                ShoppingCartItem = addToCart.shoppingCartItem,
                Quantity = quantity,
                CartType = cartType,
                Currency = _workContext.WorkingCurrency,
                Store = _workContext.CurrentStore,
                Language = _workContext.WorkingLanguage,
                TaxDisplayType = _workContext.TaxDisplayType,
            });

            //added to the cart/wishlist
            switch (cartType)
            {
                case ShoppingCartType.Wishlist:
                    {
                        //activity log
                        _ = _customerActivityService.InsertActivity("PublicStore.AddToWishlist", product.Id,
                            _workContext.CurrentCustomer, HttpContext.Connection?.RemoteIpAddress?.ToString(),
                            _translationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

                        if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
                        {
                            //redirect to the wishlist page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("Wishlist"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var qty = (await _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, ShoppingCartType.Wishlist)).Sum(x => x.Quantity);
                        var updatetopwishlistsectionhtml = string.Format(_translationService.GetResource("Wishlist.HeaderQuantity"), qty);

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_translationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
                            updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
                            wishlistqty = qty,
                            model = addtoCartModel
                        });
                    }
                case ShoppingCartType.ShoppingCart:
                default:
                    {
                        //activity log
                        _ = _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", product.Id,
                            _workContext.CurrentCustomer, HttpContext.Connection?.RemoteIpAddress?.ToString(),
                            _translationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

                        if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
                        {
                            //redirect to the shopping cart page
                            return Json(new
                            {
                                redirect = Url.RouteUrl("ShoppingCart"),
                            });
                        }

                        //display notification message and update appropriate blocks
                        var shoppingCartTypes = new List<ShoppingCartType>();
                        shoppingCartTypes.Add(ShoppingCartType.ShoppingCart);
                        shoppingCartTypes.Add(ShoppingCartType.Auctions);
                        if (_shoppingCartSettings.AllowOnHoldCart)
                            shoppingCartTypes.Add(ShoppingCartType.OnHoldCart);

                        var updatetopcartsectionhtml = string.Format(_translationService.GetResource("ShoppingCart.HeaderQuantity"),
                            (await _shoppingCartService.GetShoppingCart(_workContext.CurrentStore.Id, shoppingCartTypes.ToArray()))
                                .Sum(x => x.Quantity));

                        var miniShoppingCartmodel = _shoppingCartSettings.MiniShoppingCartEnabled ? await _mediator.Send(new GetMiniShoppingCart() {
                            Customer = _workContext.CurrentCustomer,
                            Currency = _workContext.WorkingCurrency,
                            Language = _workContext.WorkingLanguage,
                            TaxDisplayType = _workContext.TaxDisplayType,
                            Store = _workContext.CurrentStore
                        }) : null;

                        return Json(new
                        {
                            success = true,
                            message = string.Format(_translationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
                            updatetopcartsectionhtml = updatetopcartsectionhtml,
                            sidebarshoppingcartmodel = miniShoppingCartmodel,
                            model = addtoCartModel
                        });
                    }
            }
        }



        protected IActionResult RedirectToProduct(Product product, ShoppingCartType cartType, int quantity
                , IList<CustomAttribute> currentAttributeValue)
        {
            //we can't add grouped products 
            if (product.ProductTypeId == ProductType.GroupedProduct)
            {
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }

            //products with "minimum order quantity" more than a specified qty
            if (cartType == ShoppingCartType.ShoppingCart && product.OrderMinimumQuantity > quantity)
            {
                //we cannot add to the cart such products from category pages
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }

            if (cartType == ShoppingCartType.ShoppingCart && product.EnteredPrice)
            {
                //cannot be added to the cart (requires a customer to enter price)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }
            var allowedQuantities = product.ParseAllowedQuantities();
            if (cartType == ShoppingCartType.ShoppingCart && allowedQuantities.Length > 0)
            {
                //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }

            if (cartType != ShoppingCartType.Wishlist && product.ProductAttributeMappings.Any())
            {
                var props = product.ProductAttributeMappings.ToList();
                foreach (var property in currentAttributeValue)
                {
                    props.Remove(props.FirstOrDefault( p => p.Id == property.Key));
                }


                if(props.Count()>0)
                //product has some attributes
                return Json(new
                {
                    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName(_workContext.WorkingLanguage.Id) }),
                });
            }

            return null;
        }

        protected string GetWarehouse(Product product)
        {
            return product.UseMultipleWarehouses ? _workContext.CurrentStore.DefaultWarehouseId :
               (string.IsNullOrEmpty(_workContext.CurrentStore.DefaultWarehouseId) ? product.WarehouseId : _workContext.CurrentStore.DefaultWarehouseId);
        }
    }
}
