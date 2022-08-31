using Automatonymous;
using Grand.SharedKernel;
using Newtonsoft.Json;
using Payments.BahamtaIR.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Payments.BahamtaIR.Services
{
    public class BahamtaIrHttpClient: IBahamtaIrHttpClient
    {
        private readonly HttpClient _client;
        public BahamtaIrHttpClient(HttpClient client) {
            _client = client;
        }

        public async Task<BahamtaCreatePaymentResponse> CreateBahamtaPayment(string apiKey, BahamtaDataNeedForCreatePayment request, bool isTest)
        {
            var checkRequest = request.check();
            if (checkRequest.hasError)
                return new BahamtaCreatePaymentResponse {
                    PaymentLink = string.Empty,
                    HasError = true,
                    ErrorMessage = checkRequest.errorMessage
                };



            addHttpHeaders(this._client);

            string url = __createUrl(BahamtaIRHelper.BahamtaCreatePaymentURL, apiKey , request);

            var getAnswer = await get(this._client, url);


            switch (getAnswer.response.StatusCode)
            {
                //200
                case System.Net.HttpStatusCode.OK:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(getAnswer.content, 
                                new { 
                                    ok = true,
                                    error="error key",
                                    result = new { 
                                        payment_url= "" 
                                    } 
                                });

                        if (jsonAnswer == null)
                        {
                            return new BahamtaCreatePaymentResponse {
                                HasError = true,
                                ErrorMessage = "Invalid response: " + getAnswer.content
                            };
                        }

                        if (!jsonAnswer.ok)
                        {
                            return new BahamtaCreatePaymentResponse {
                                HasError = true,
                                ErrorMessage = _foundPaymentRequestError(jsonAnswer.error)
                            };
                        }

                        if (jsonAnswer.result == null || jsonAnswer.result.payment_url == null)
                        {
                            return new BahamtaCreatePaymentResponse {
                                HasError = true,
                                ErrorMessage = "Invalid response: " + getAnswer.content
                            };
                        }

                        return new BahamtaCreatePaymentResponse {
                            PaymentLink = jsonAnswer.result.payment_url,
                            HasError = false,
                            ErrorMessage = string.Empty
                        };

                    }

                default:
                    {
                        return new BahamtaCreatePaymentResponse {
                            PaymentLink = string.Empty,
                            HasError = true,
                            ErrorMessage = "service request error " + getAnswer.response.RequestMessage
                        };
                    }
            }

            

            static string __createUrl(string baseUrl, string apiKey, BahamtaDataNeedForCreatePayment request)
            {
                var sb = new StringBuilder(baseUrl);
                sb.Append($"?api_key={apiKey}");
                sb.Append($"&reference={request.order_id}");
                sb.Append($"&amount_irr={request.amount}");
                sb.Append($"&payer_mobile={request.mail}");
                sb.Append($"&callback_url={request.callback}");
                
                return sb.ToString();
            }

            
        }


        public async Task<BahamtaVerifyPaymentRespons> VerifyPayment(string apiKey, string order_id, string paymemntAmountIRR)
        {
            addHttpHeaders(this._client);
            string url = __createUrl(BahamtaIRHelper.BahamtaVerifyPaymentURL, apiKey, order_id, paymemntAmountIRR);
            var postAnswer = await get(this._client, url);


            switch (postAnswer.response.StatusCode)
            {
                //200
                case System.Net.HttpStatusCode.OK:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(postAnswer.content, 
                            new { 
                                ok=true,
                                error = "<ERROR_KEY>",
                                result = new
                                {
                                    state= "paid",
                                    total= 1000000,
                                    wage= 5000,
                                    gateway= "sep",
                                    terminal= "11223344",
                                    pay_ref= "GmshtyjwKSu5lKOLquYrzO9BqjUMb/TPUK0qak/iVs",
                                    pay_trace= "935041",
                                    pay_pan= "123456******1234",
                                    pay_cid= "77CB1B455FB5F60415A7A02E4502134CFD72DBF6D1EC8FA2B48467DFB124AA75A",
                                    pay_time= "2019-11-12T16:39:57.686436+03:30"
                                }
                            });
                        if (jsonAnswer == null)
                        {
                            throw new GrandException("پاسخ غیر متعارف از باهمتا هنگام تایید پرداخت - " + postAnswer.content);
                        }
                        if (!jsonAnswer.ok)
                        {
                            if(jsonAnswer.error==null) throw new GrandException("پاسخ غیر متعارف از باهمتا هنگام تایید پرداخت - " + postAnswer.content);

                            return new BahamtaVerifyPaymentRespons {
                                PaymentPerformed = false,
                                HasError = true,
                                ErrorMessage = _foundVerifyRequestError(jsonAnswer.error),
                            };

                        }

                        if (jsonAnswer.result == null || jsonAnswer.result.pay_trace==null)
                        {
                            throw new GrandException("پاسخ غیر متعارف از باهمتا هنگام تایید پرداخت - " + postAnswer.content);
                        }

                        return new BahamtaVerifyPaymentRespons {
                            PaymentPerformed = true,
                            OrderId = order_id,
                            PaymentTransactionId = jsonAnswer.result.pay_trace,
                            HasError = false,
                            ErrorMessage = String.Empty
                        };
                    }

                default:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(postAnswer.content, new { error_code = "", error_message = "" });
                        return new BahamtaVerifyPaymentRespons {
                            PaymentPerformed = false,
                            HasError = true,
                            ErrorMessage = jsonAnswer!.error_message
                        };
                    }
            }

            static string __createUrl(string baseUrl, string api_key, string order_id, string paymemntAmountIRR)
            {
                var sb = new StringBuilder(baseUrl);
                sb.Append($"?api_key={api_key}");
                sb.Append($"&reference={order_id}");
                sb.Append($"&amount_irr={paymemntAmountIRR}");
                return sb.ToString();
            }
        }


        private static void addHttpHeaders(HttpClient http)
        {
            http.DefaultRequestHeaders.Add(name: "user-agent", value: "Webpay on Grandnode2");
        }

        private static async Task<(HttpResponseMessage response, string content)> get(HttpClient http, string url)
        {
            
            HttpResponseMessage answer = await http.GetAsync(requestUri: url);
            var responseContent = await answer.Content.ReadAsStringAsync();

            return (response: answer, content: responseContent);
        }


        private string _foundPaymentRequestError(string errorCode)
        {
            var errors = _getBahamtaPaymentRequestErrors();
            var found = errors.Find(f=>f.ErrorKey.Equals(errorCode,StringComparison.CurrentCultureIgnoreCase));
            if (found != null) return found.ErrorDescription;
            return errorCode;
        }
        private List<BahamtaServiceErrorCode> _getBahamtaPaymentRequestErrors()
        {
            return new List<BahamtaServiceErrorCode> {
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_API_CALL",
                   ErrorDescription="قالب فراخوانی سرویس رعایت نشده است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_API_KEY",
                   ErrorDescription="کلید الکترونیکی صاحب فروشگاه فرستاده نشده و یا ساختار آن نادرست است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="NOT_AUTHORIZED",
                   ErrorDescription="گرچه ساختار کلید درست است، اما هیچ فروشنده‌ای با این کلید در وب پی ثبت نشده است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_AMOUNT",
                   ErrorDescription="پارامتر مبلغ فرستاده نشده و یا نادرست فرستاده شده است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="LESS_THAN_WAGE_AMOUNT",
                   ErrorDescription="مبلغ کمتر از کارمزد پرداختی است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="TOO_LESS_AMOUNT",
                   ErrorDescription="مبلغ کمتر از حد مجاز (هزار تومان) است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="TOO_MUCH_AMOUNT",
                   ErrorDescription="مبلغ بیشتر از حد مجاز (پنجاه میلیون تومان) است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_REFERENCE",
                   ErrorDescription="شماره شناسه پرداخت ناردست است. این مقدار باید یک عبارت حرفی با طول بین ۱ تا ۶۴ حرف باشد.",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_TRUSTED_PAN",
                   ErrorDescription="لیست شماره کارتها نادرست است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_CALLBACK",
                   ErrorDescription="آدرس فراخوانی نادرست است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="INVALID_PARAM",
                   ErrorDescription="خطایی در مقادیر فرستاده شده وجود دارد که جزو موارد شناخته شده نیست",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="ALREADY_PAID",
                   ErrorDescription="درخواست پرداختی با شناسه داده شده قبلاً ثبت و پرداخت شده است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="MISMATCHED_DATA",
                   ErrorDescription="درخواست پرداختی با شناسه داده شده قبلاً ثبت و منتظر پرداخت است، اما مقادیر فرستاده شده در این درخواست، با درخواست اصلی متفاوت است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="NO_REG_TERMINAL",
                   ErrorDescription="ترمینالی برای این فروشنده ثبت نشده است",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="NO_AVAILABLE_GATEWAY",
                   ErrorDescription="رگاههای پرداختی که این فروشنده در آنها ترمینال ثبت شده دارد، قادر به ارائه خدمات نیستند",
               },
               new BahamtaServiceErrorCode {
                   ErrorKey="SERVICE_ERROR",
                   ErrorDescription="خطای داخلی سرویس رخ داده است",
               },
            };
        }
        private class BahamtaServiceErrorCode
        {
            public string ErrorKey { get; set; }
            public string ErrorDescription { get; set; }
        }


        private string _foundVerifyRequestError(string errorCode)
        {
            var errors = _getVerifyRequestErrors();
            var found = errors.Find(f => f.ErrorKey.Equals(errorCode, StringComparison.CurrentCultureIgnoreCase));
            if (found != null) return found.ErrorDescription;
            return errorCode;
        }
        private List<BahamtaServiceErrorCode> _getVerifyRequestErrors()
        {
            return new List<BahamtaServiceErrorCode> {
                new BahamtaServiceErrorCode {
                    ErrorKey="NOT_AUTHORIZED",
                    ErrorDescription = "گرچه ساختار کلید درست است، اما هیچ فروشنده‌ای با این کلید در وب پی ثبت نشده است",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="INVALID_AMOUNT",
                    ErrorDescription = "پارامتر مبلغ فرستاده نشده و یا نادرست فرستاده شده است",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="INVALID_REFERENCE",
                    ErrorDescription = "شماره شناسه پرداخت ناردست است",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="INVALID_PARAM",
                    ErrorDescription = "خطایی در مقادیر فرستاده شده وجود دارد که جزو موارد ثبت شده نیست",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="UNKNOWN_BILL",
                    ErrorDescription = "پرداختی با شماره شناسه فرستاده شده ثبت نشده است",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="MISMATCHED_DATA",
                    ErrorDescription = "مبلغ اعلام شده با آنچه در وب پی ثبت شده است مطابقت ندارد",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="NOT_CONFIRMED",
                    ErrorDescription = "این پرداخت تأیید نشد",
                },
                new BahamtaServiceErrorCode {
                    ErrorKey="SERVICE_ERROR",
                    ErrorDescription = "خطای داخلی سرویس رخ داده است",
                },

            };
        }

    }
}
