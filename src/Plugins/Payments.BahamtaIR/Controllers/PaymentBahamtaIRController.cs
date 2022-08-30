using Grand.Business.Checkout.Commands.Models.Orders;
using Grand.Business.Checkout.Extensions;
using Grand.Business.Checkout.Interfaces.Orders;
using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Business.Checkout.Queries.Models.Orders;
using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Logging;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Infrastructure;
using Grand.SharedKernel;
using Grand.Web.Common.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Payments.BahamtaIR.Services;
using System.Globalization;

namespace Payments.BahamtaIR.Controllers
{

    public class PaymentBahamtaIRController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;
        private readonly IMediator _mediator;
        private readonly IPaymentTransactionService _paymentTransactionService;
        private readonly IBahamtaIrHttpClient _BahamtaHttpClient;

        private readonly PaymentSettings _paymentSettings;
        private readonly BahamtaIRPaymentSettings _bahamtaIRPaymentSettings;

        public PaymentBahamtaIRController(
            IWorkContext workContext,
            IPaymentService paymentService,
            IOrderService orderService,
            ILogger logger,
            IMediator mediator,
            IPaymentTransactionService paymentTransactionService,
            IBahamtaIrHttpClient paypalHttpClient,
            BahamtaIRPaymentSettings payPalStandardPaymentSettings,
            PaymentSettings paymentSettings)
        {
            _workContext = workContext;
            _paymentService = paymentService;
            _orderService = orderService;
            _logger = logger;
            _mediator = mediator;
            _paymentTransactionService = paymentTransactionService;
            _BahamtaHttpClient = paypalHttpClient;
            _bahamtaIRPaymentSettings = payPalStandardPaymentSettings;
            _paymentSettings = paymentSettings;
        }


        private string QueryString(string name)
        {
            if (HttpContext.Request.Method == "POST")
            {
               if( StringValues.IsNullOrEmpty(HttpContext.Request.Form[name]))
                    return default;
                return HttpContext.Request.Form[name].ToString();
            }


            if (StringValues.IsNullOrEmpty(HttpContext.Request.Query[name]))
                return default;

            return HttpContext.Request.Query[name].ToString();
        }


        /// <summary>
        /// پرداخت موفقیت آمیز بود، پی پال برمی گردونه به این آدرس
        /// </summary>
        /// <returns></returns>
        /// <exception cref="GrandException"></exception>
        public async Task<IActionResult> PaymentHandler()
        {
            
            var status = QueryString("status");
            var track_id = QueryString("track_id");
            var transactionId = QueryString("id");
            var order_id = QueryString("order_id");

            if (_paymentService.LoadPaymentMethodBySystemName("Payments.BahamtaIR") is not BahamtaIRPaymentProvider processor ||
                !processor.IsPaymentMethodActive(_paymentSettings))
                throw new GrandException("Bahamta.Ir module cannot be loaded");

            BahamtaVerifyPaymentRespons response = 
                await _BahamtaHttpClient.VerifyPayment(this._bahamtaIRPaymentSettings.ApiToken, transactionId, order_id,isTest:false);

            if (response.HasError) throw new Exception(response.ErrorMessage);

            if (!response.PaymentPerformed) throw new Exception("پرداخت انجام نشده است" + response.ErrorMessage);
            if (response.OrderId != order_id || response.PaymentTransactionId != transactionId) throw new Exception("بین درخواست و پاسخ همخوانی وجود ندارد");

            var orderNumberGuid = new Guid(order_id);
            Order order = await _orderService.GetOrderByGuid(orderNumberGuid);
            var paymentTransaction = await _paymentTransactionService.GetByOrdeGuid(orderNumberGuid);

            var sb = new StringBuilder();
            sb.AppendLine("TransactionId:" + order_id);
            sb.AppendLine("track_id:" + track_id);

            //order note
            await _orderService.InsertOrderNote(new OrderNote {
                Note = sb.ToString(),
                DisplayToCustomer = false,
                CreatedOnUtc = DateTime.UtcNow,
                OrderId = order.Id,
            });

            if (await _mediator.Send(new CanMarkPaymentTransactionAsPaidQuery() { PaymentTransaction = paymentTransaction }))
            {
                paymentTransaction.AuthorizationTransactionId = order_id;
                await _paymentTransactionService.UpdatePaymentTransaction(paymentTransaction);
                await _mediator.Send(new MarkAsPaidCommand() { PaymentTransaction = paymentTransaction });
            }

            return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
        }

        
    }
}