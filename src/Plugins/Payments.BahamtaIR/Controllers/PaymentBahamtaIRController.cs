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
using Payments.BahamtaIR.Helper;
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
            if (_paymentService.LoadPaymentMethodBySystemName("Payments.BahamtaIR") is not BahamtaIRPaymentProvider processor ||
                !processor.IsPaymentMethodActive(_paymentSettings))
                throw new GrandException("Bahamta.Ir module cannot be loaded");

            var status = QueryString("state");
            var order_id = QueryString("reference");

            if(status== "error")
            {
                var errorMessage = QueryString("error_message");
                throw new GrandException("خطا در پرداخت - " + errorMessage);
            }

            if(status != "wait_for_confirm")
            {
                throw new GrandException("خطا در پرداخت - وضعیت باید منتظر تایید باشد current status: " + status);
            }



            

            var orderNumberGuid = new Guid(order_id);
            Order order = await _orderService.GetOrderByGuid(orderNumberGuid);
            if (order == null) throw new GrandException("سفارش یافت نشد");
            
            var paymentTransaction = await _paymentTransactionService.GetByOrdeGuid(orderNumberGuid);
            if (paymentTransaction.TransactionStatus == TransactionStatus.Paid)
            {
                throw new GrandException("سفارش در حال حاضر پرداخت شده است");
            }

            //--------------
            BahamtaVerifyPaymentRespons response =
                await _BahamtaHttpClient.VerifyPayment(this._bahamtaIRPaymentSettings.ApiToken, order_id, order.MyRoundTotalAmountOfOrder());

            if (response.HasError) throw new GrandException(response.ErrorMessage);

            if (!response.PaymentPerformed) throw new GrandException("پرداخت انجام نشده است" + response.ErrorMessage);
            if (response.OrderId != order_id) throw new GrandException("بین درخواست و پاسخ همخوانی وجود ندارد");
            //--------------

            var sb = new StringBuilder();
            sb.AppendLine("ReferenceId:" + order_id);
            sb.AppendLine("track_id:" + response.PaymentTransactionId);

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