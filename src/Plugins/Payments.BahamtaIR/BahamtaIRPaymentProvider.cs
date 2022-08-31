using Grand.Business.Catalog.Interfaces.Products;
using Grand.Business.Catalog.Interfaces.Tax;
using Grand.Business.Checkout.Enum;
using Grand.Business.Checkout.Interfaces.CheckoutAttributes;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Utilities;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Customers.Interfaces;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Domain.Shipping;
using Grand.Infrastructure;
using Grand.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Payments.BahamtaIR.Helper;
using Payments.BahamtaIR.Services;
using System.Globalization;

namespace Payments.BahamtaIR
{
    public class BahamtaIRPaymentProvider : IPaymentProvider
    {

        private readonly ITranslationService _translationService;
        private readonly BahamtaIRPaymentSettings _bahamtaIRPaymentSettings;

        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly IUserFieldService _userFieldService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITaxService _taxService;
        private readonly IProductService _productService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWorkContext _workContext;
        private readonly IOrderService _orderService;
        private readonly IBahamtaIrHttpClient _bahamtaHttpClient;

        #region Ctor

        public BahamtaIRPaymentProvider(
            ICheckoutAttributeParser checkoutAttributeParser,
            IUserFieldService userFieldService,
            IHttpContextAccessor httpContextAccessor,
            ITranslationService translationService,
            ITaxService taxService,
            IProductService productService,
            IServiceProvider serviceProvider,
            IWorkContext workContext,
            IOrderService orderService,
            BahamtaIRPaymentSettings bahamtaIRPaymentSettings,
            IBahamtaIrHttpClient bahamtaHttpClient)
        {
            _checkoutAttributeParser = checkoutAttributeParser;
            _userFieldService = userFieldService;
            _httpContextAccessor = httpContextAccessor;
            _translationService = translationService;
            _taxService = taxService;
            _productService = productService;
            _serviceProvider = serviceProvider;
            _workContext = workContext;
            _orderService = orderService;
            _bahamtaIRPaymentSettings = bahamtaIRPaymentSettings;
            _bahamtaHttpClient = bahamtaHttpClient;
        }

        #endregion

        public string ConfigurationUrl => BahamtaIRPaymentDefaults.ConfigurationUrl;

        public string SystemName => BahamtaIRPaymentDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(BahamtaIRPaymentDefaults.FriendlyName);

        public int Priority => _bahamtaIRPaymentSettings.DisplayOrder;

        public IList<string> LimitedToStores => new List<string>();

        public IList<string> LimitedToGroups => new List<string>();


        #region Utilities

        

        private BahamtaDataNeedForCreatePayment createBahamtaRequest(Order order)
        {
            //get store location
            var storeLocation = _workContext.CurrentHost.Url.TrimEnd('/');

            return new BahamtaDataNeedForCreatePayment {
                order_id = order.OrderGuid.ToString(),
                amount = order.MyRoundTotalAmountOfOrder(),
                desc= "جهت درخواست شماره "+ order.OrderNumber.ToString(),
                mail= order.ShippingAddress?.Email,
                name= order.ShippingAddress?.FirstName + " " + order.ShippingAddress?.LastName,
                phone=order.ShippingAddress?.PhoneNumber,
                callback= $"{storeLocation}/Plugins/PaymentBahamtaIR/PaymentHandler"
            };
        }

        

        #endregion

        #region Properties

        /// <summary>
        /// Init a process a payment transaction
        /// </summary>
        /// <returns>Payment transaction</returns>
        public async Task<PaymentTransaction> InitPaymentTransaction()
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public async Task<ProcessPaymentResult> ProcessPayment(PaymentTransaction paymentTransaction)
        {
            var result = new ProcessPaymentResult();
            return await Task.FromResult(result);
        }
        public Task PostProcessPayment(PaymentTransaction paymentTransaction)
        {
            //nothing
            return Task.CompletedTask;
        }
        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public async Task PostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            var order = await _orderService.GetOrderByGuid(paymentTransaction.OrderGuid);
            
            BahamtaDataNeedForCreatePayment request = createBahamtaRequest(order);
            var isTest = false; // this._bahamtaIRPaymentSettings.UseSandbox;

            var response = await this._bahamtaHttpClient
                                .CreateBahamtaPayment(this._bahamtaIRPaymentSettings.ApiToken, request, isTest:isTest);

            if (response.HasError) throw new GrandException(response.ErrorMessage);

            await _userFieldService.SaveField(order, "BahamtaPayment_Id_Link", response.PaymentLink);

            _httpContextAccessor.HttpContext.Response.Redirect(response.PaymentLink);
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public async Task<bool> HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public async Task<double> GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;

            //if (_bahamtaIRPaymentSettings.AdditionalFee <= 0)
            //    return _bahamtaIRPaymentSettings.AdditionalFee;

            //double result;
            //if (_bahamtaIRPaymentSettings.AdditionalFeePercentage)
            //{
            //    //percentage
            //    var orderTotalCalculationService = _serviceProvider.GetRequiredService<IOrderCalculationService>();
            //    var subtotal = await orderTotalCalculationService.GetShoppingCartSubTotal(cart, true);
            //    result = (double)((((float)subtotal.subTotalWithDiscount) * ((float)_bahamtaIRPaymentSettings.AdditionalFee)) / 100f);
            //}
            //else
            //{
            //    //fixed value
            //    result = _bahamtaIRPaymentSettings.AdditionalFee;
            //}
            //if (result > 0)
            //{
            //    var currencyService = _serviceProvider.GetRequiredService<ICurrencyService>();
            //    result = await currencyService.ConvertFromPrimaryStoreCurrency(result, _workContext.WorkingCurrency);
            //}
            ////return result;
            //return await Task.FromResult(result);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <returns>Capture payment result</returns>
        public async Task<CapturePaymentResult> Capture(PaymentTransaction paymentTransaction)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public async Task<RefundPaymentResult> Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <returns>Result</returns>
        public async Task<VoidPaymentResult> Void(PaymentTransaction paymentTransaction)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return await Task.FromResult(result);
        }

        /// <summary>
        /// Cancel a payment
        /// </summary>
        /// <returns>Result</returns>
        public Task CancelPayment(PaymentTransaction paymentTransaction)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <returns>Result</returns>
        public async Task<bool> CanRePostRedirectPayment(PaymentTransaction paymentTransaction)
        {
            if (paymentTransaction == null)
                throw new ArgumentNullException(nameof(paymentTransaction));

            //ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - paymentTransaction.CreatedOnUtc).TotalSeconds < 15)
                return false;

            return await Task.FromResult(true);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public async Task<IList<string>> ValidatePaymentForm(IFormCollection form)
        {
            return await Task.FromResult(new List<string>());
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public async Task<PaymentTransaction> SavePaymentInfo(IFormCollection form)
        {
            return await Task.FromResult<PaymentTransaction>(null);
        }


        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public async Task<bool> SupportCapture()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public async Task<bool> SupportPartiallyRefund()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public async Task<bool> SupportRefund()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public async Task<bool> SupportVoid()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public async Task<bool> SkipPaymentInfo()
        {
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public async Task<string> Description()
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to PayPal site to complete the payment"
            return await Task.FromResult(_translationService.GetResource("Plugins.Payments.BahamtaIR.PaymentMethodDescription"));
        }

        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentBahamtaIR";
        }

        public string LogoURL => "/Plugins/Payments.BahamtaIR/logo.jpg";

        #endregion
    }
}
