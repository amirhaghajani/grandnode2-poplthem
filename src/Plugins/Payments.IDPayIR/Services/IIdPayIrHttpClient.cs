﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.IDPayIR.Services
{
    public interface IIdPayIrHttpClient
    {
        Task<IdPayCreatePaymentResponse> CreateIdPayPayment(string apiKey, IdPayDataNeedForCreatePayment request, bool isTest);

        Task<IdPayVerifyPaymentRespons> VerifyPayment(string apiKey, string paymentTransactionId, string order_id, bool isTest);
    }

    public class IdPayDataNeedForCreatePayment
    {
        public string order_id { get; set; } = "";

        /// <summary>
        /// must be Rial
        /// </summary>
        public string amount { get; set; } = "0";

        /// <summary>
        /// نام پرداخت کننده
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// تلفن همراه پرداخت کننده
        /// </summary>
        public string phone { get; set; } = "";

        /// <summary>
        /// ایمیل پرداخت کننده
        /// </summary>
        public string mail { get; set; } = "";

        /// <summary>
        /// توضیحات تراکنش حداکثر 255 کاراکتر
        /// </summary>
        public string desc { get; set; } = "";

        /// <summary>
        /// آدرس برگشت حداکثر 2048
        /// </summary>
        public string callback { get; set; } = "";

        public (bool hasError, string errorMessage) check()
        {
            if (string.IsNullOrWhiteSpace(order_id))
                return (hasError: true, errorMessage: "order_id cant be empty");

            if (!int.TryParse(amount, out int amountInt))
                return (hasError: true, errorMessage: "amount number is not correct");

            if (amountInt < 1000 || amountInt > 500000000)
                return (hasError: true, errorMessage: "amount must be 1,000 to 500,000,000");


            if (desc.Length > 255)
                return (hasError: true, errorMessage: "description must be less 256");

            if (callback.Length > 2048)
                return (hasError: true, errorMessage: "callback must be less 2049");

            return (hasError: false, errorMessage: string.Empty);
        }
    }

    public class IdPayCreatePaymentResponse
    {
        public string PaymentTransactionId { get; set; } = "";
        public string PaymentLink { get; set; } = "";
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

    }

    public class IdPayVerifyPaymentRespons
    {
        public bool PaymentPerformed { get; set; }
        public string OrderId { get; set; }
        public string PaymentTransactionId { get; set; } = "";

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }

    }
}