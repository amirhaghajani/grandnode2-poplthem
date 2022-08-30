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
                                new { ok = true,
                                    error="error key",
                                    result = new { 
                                        payment_url="" 
                                    } 
                                });

                        if (jsonAnswer == null)
                        {
                            return new BahamtaCreatePaymentResponse {
                                HasError = true,
                                ErrorMessage = "Invalid response: " + getAnswer.content
                            };
                        }

                        if (jsonAnswer.ok)
                        {
                            return new BahamtaCreatePaymentResponse {
                                PaymentLink = jsonAnswer!.result.payment_url,
                                HasError = false,
                                ErrorMessage = string.Empty
                            };
                        }

                        return new BahamtaCreatePaymentResponse {
                            HasError = true,
                            ErrorMessage = _foundPaymentRequestError(jsonAnswer.error)
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
                sb.Append($"?reference={request.order_id}");
                sb.Append($"?amount_irr={request.amount}");
                sb.Append($"?payer_mobile={request.mail}");
                sb.Append($"?callback_url={request.callback}");
                
                return sb.ToString();
            }

            
        }


        public async Task<BahamtaVerifyPaymentRespons> VerifyPayment(string apiKey, string paymentTransactionId, string order_id, bool isTest)
        {
            addHttpHeaders(this._client, apiKey, isTest);
            string content = __createJsonContent(paymentTransactionId, order_id);
            var postAnswer = await post(this._client, BahamtaIRHelper.BahamtaVerifyPaymentURL, content);


            switch (postAnswer.response.StatusCode)
            {
                //200
                case System.Net.HttpStatusCode.OK:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(postAnswer.content, 
                            new { status = 1,
                                track_id = "",
                                id="",
                                order_id="",
                                amount="",
                                date="",
                                payment=new
                                {
                                    track_id="",
                                    amount="",
                                    card_no="",
                                    hashed_card_no="",
                                    date=""
                                },
                                verify = new
                                {
                                    date=""
                                }
                            });
                        var status = getVerifyStatusCode().FirstOrDefault(s => s.StatusCode == jsonAnswer.status);
                        if (status == null)
                            return new BahamtaVerifyPaymentRespons {
                                PaymentPerformed=false,
                                HasError=true,
                                ErrorMessage= "Unknown response status code"
                            };
                            
                        
                        if(!status.IsPaymentPerformed)
                            return new BahamtaVerifyPaymentRespons {
                                PaymentPerformed = false,
                                HasError = true,
                                ErrorMessage = status.Description
                            };

                        return new BahamtaVerifyPaymentRespons {
                            PaymentPerformed = true,
                            OrderId=jsonAnswer.order_id,
                            PaymentTransactionId = jsonAnswer.id,
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

            static string __createJsonContent(string paymentTransactionId, string order_id)
            {
                var contect = new {id= paymentTransactionId, order_id= order_id };
                string jsonSerialized = JsonConvert.SerializeObject(contect);
                return jsonSerialized;
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


        private List<BahamtaVerifyPaymentStatusCode> getVerifyStatusCode()
        {
            return new List<BahamtaVerifyPaymentStatusCode> {
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 1,
                    Description="پرداخت انجام نشده است",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 2,
                    Description="پرداخت ناموفق بوده است",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 3,
                    Description="خطا رخ داده است",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 4,
                    Description="بلوکه شده",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 5,
                    Description="برگشت به پرداخت کننده",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 6,
                    Description="برگشت خورده سیستمی",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 7,
                    Description="انصراف از پرداخت",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 8,
                    Description="به درگاه پرداخت منتقل شد",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 10,
                    Description="در انتظار تایید پرداخت",
                    IsPaymentPerformed=false
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 100,
                    Description="پرداخت تایید شده است",
                    IsPaymentPerformed=true
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 101,
                    Description="پرداخت قبلا تایید شده است",
                    IsPaymentPerformed=true
                },
                new BahamtaVerifyPaymentStatusCode {
                    StatusCode = 200,
                    Description="به دریافت کننده واریز شد",
                    IsPaymentPerformed=true
                },
            };
        }

        private class BahamtaVerifyPaymentStatusCode
        {
            public int StatusCode { get; set; }
            public string Description { get; set; }
            public bool IsPaymentPerformed { get; set; }
        }
    }
}
