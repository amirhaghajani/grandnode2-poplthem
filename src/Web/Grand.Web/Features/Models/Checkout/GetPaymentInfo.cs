﻿using Grand.Business.Checkout.Interfaces.Payments;
using Grand.Web.Models.Checkout;
using MediatR;

namespace Grand.Web.Features.Models.Checkout
{
    public class GetPaymentInfo : IRequest<CheckoutPaymentInfoModel>
    {
        public IPaymentProvider PaymentMethod { get; set; }
    }
}
