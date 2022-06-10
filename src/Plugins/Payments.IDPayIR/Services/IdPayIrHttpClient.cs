using Newtonsoft.Json;
using Payments.IDPayIR.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Payments.IDPayIR.Services
{
    public class IdPayIrHttpClient: IIdPayIrHttpClient
    {
        private readonly HttpClient _client;
        public IdPayIrHttpClient(HttpClient client) {
            _client = client;
        }

        public async Task<IdPayCreatePaymentResponse> CreateIdPayPayment(string apiKey, IdPayDataNeedForCreatePayment request, bool isTest)
        {
            var checkRequest = request.check();
            if (checkRequest.hasError)
                return new IdPayCreatePaymentResponse {
                    PaymentTransactionId = string.Empty,
                    PaymentLink = string.Empty,
                    HasError = true,
                    ErrorMessage = checkRequest.errorMessage
                };



            addHttpHeaders(this._client, apiKey, isTest);

            string content = __createJsonContent(request);

            var postAnswer = await post(this._client, IDPayIRHelper.IdPayCreatePaymentURL, content);


            switch (postAnswer.response.StatusCode)
            {
                //201
                case System.Net.HttpStatusCode.Created:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(postAnswer.content, new { id = "", link = "" });
                        return new IdPayCreatePaymentResponse {
                            PaymentTransactionId = jsonAnswer!.id,
                            PaymentLink = jsonAnswer!.link,
                            HasError = false,
                            ErrorMessage = string.Empty
                        };
                    }

                default:
                    {
                        var jsonAnswer = JsonConvert.DeserializeAnonymousType(postAnswer.content, new { error_code = "", error_message = "" });
                        return new IdPayCreatePaymentResponse {
                            PaymentTransactionId = string.Empty,
                            PaymentLink = string.Empty,
                            HasError = true,
                            ErrorMessage = jsonAnswer!.error_message
                        };
                    }
            }

            

            static string __createJsonContent(IdPayDataNeedForCreatePayment request)
            {
                string jsonSerialized = JsonConvert.SerializeObject(request);
                return jsonSerialized;
            }

            
        }


        public async Task<IdPayVerifyPaymentRespons> VerifyPayment(string apiKey, string paymentTransactionId, string order_id, bool isTest)
        {
            addHttpHeaders(this._client, apiKey, isTest);
            string content = __createJsonContent(paymentTransactionId, order_id);
            var postAnswer = await post(this._client, IDPayIRHelper.IdPayVerifyPaymentURL, content);


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
                            return new IdPayVerifyPaymentRespons {
                                PaymentPerformed=false,
                                HasError=true,
                                ErrorMessage= "Unknown response status code"
                            };
                            
                        
                        if(!status.IsPaymentPerformed)
                            return new IdPayVerifyPaymentRespons {
                                PaymentPerformed = false,
                                HasError = true,
                                ErrorMessage = status.Description
                            };

                        return new IdPayVerifyPaymentRespons {
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
                        return new IdPayVerifyPaymentRespons {
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


        private static void addHttpHeaders(HttpClient http, string apiKey, bool isTest)
        {
            http.DefaultRequestHeaders.Add(name: "X-API-KEY", value: apiKey);
            if (isTest) http.DefaultRequestHeaders.Add(name: "X-SANDBOX", value: "1");
        }

        private static async Task<(HttpResponseMessage response, string content)> post(HttpClient http, string url, string content)
        {
            HttpResponseMessage answer = await http.PostAsync(
            requestUri: url,
            content: new StringContent(content, System.Text.Encoding.UTF8, "application/json")
            );
            var responseContent = await answer.Content.ReadAsStringAsync();

            return (response: answer, content: responseContent);
        }

        private List<IdPayServiceErrorCode> _getIdPayErrors()
        {
            return new List<IdPayServiceErrorCode> {
               new IdPayServiceErrorCode { HttpStatusCode=null, StatusCode =-1, ErrorCode=-1, ErrorDescription="خطای غیر منتظره" ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Forbidden, StatusCode =403, ErrorCode=11, ErrorDescription="کاربر مسدود شده است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Forbidden,StatusCode=403, ErrorCode=12, ErrorDescription="API Key یافت نشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Forbidden,StatusCode=403, ErrorCode=13, ErrorDescription="درخواست شما از {ip} ارسال شده است. این IP با IP های ثبت شده در وب سرویس همخوانی ندارد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Forbidden,StatusCode=403, ErrorCode=14, ErrorDescription="وب سرویس شما در حال بررسی است و یا تایید نشده است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.InternalServerError,StatusCode=500, ErrorCode=15, ErrorDescription="سرویس مورد نظر در دسترس نمی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Forbidden,StatusCode=403, ErrorCode=21, ErrorDescription="حساب بانکی متصل به وب سرویس تایید نشده است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotFound,StatusCode=404, ErrorCode=22, ErrorDescription="وب سریس یافت نشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.Unauthorized,StatusCode=401, ErrorCode=23, ErrorDescription="اعتبار سنجی وب سرویس ناموفق بود." ,} ,
                new IdPayServiceErrorCode {HttpStatusCode=System.Net.HttpStatusCode.Forbidden, StatusCode=403, ErrorCode=24, ErrorDescription="حساب بانکی مرتبط با این وب سرویس غیر فعال شده است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=31, ErrorDescription="کد تراکنش id نباید خالی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=32, ErrorDescription="شماره سفارش order_id نباید خالی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=33, ErrorDescription="مبلغ amount نباید خالی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=34, ErrorDescription="مبلغ amount باید بیشتر از {min-amount} ریال باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=35, ErrorDescription="مبلغ amount باید کمتر از {max-amount} ریال باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=36, ErrorDescription="مبلغ amount بیشتر از حد مجاز است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=37, ErrorDescription="آدرس بازگشت callback نباید خالی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=38, ErrorDescription="درخواست شما از آدرس {domain} ارسال شده است. دامنه آدرس بازگشت callback با آدرس ثبت شده در وب سرویس همخوانی ندارد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=39, ErrorDescription="آدرس بازگشت callback نامعتبر است." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=41, ErrorDescription="فیلتر وضعیت تراکنش ها می بایست آرایه ای (لیستی) از وضعیت های مجاز در مستندات باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=42, ErrorDescription="فیلتر تاریخ پرداخت می بایست آرایه ای شامل المنت های min و max از نوع timestamp باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=43, ErrorDescription="فیلتر تاریخ تسویه می بایست آرایه ای شامل المنت های min و max از نوع timestamp باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.NotAcceptable,StatusCode=406, ErrorCode=44, ErrorDescription="فیلتر تراکنش صحیح نمی باشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.MethodNotAllowed,StatusCode=405, ErrorCode=51, ErrorDescription="تراکنش ایجاد نشد." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.BadRequest,StatusCode=400, ErrorCode=52, ErrorDescription="استعلام نتیجه ای نداشت." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.MethodNotAllowed,StatusCode=405, ErrorCode=53, ErrorDescription="تایید پرداخت امکان پذیر نیست." ,} ,
                new IdPayServiceErrorCode { HttpStatusCode=System.Net.HttpStatusCode.MethodNotAllowed, StatusCode=405, ErrorCode=54, ErrorDescription="مدت زمان تایید پرداخت سپری شده است." ,} ,
            };
        }
        private class IdPayServiceErrorCode
        {
            public System.Net.HttpStatusCode? HttpStatusCode { get; set; }
            public int StatusCode { get; set; }
            public int ErrorCode { get; set; }
            public string? ErrorDescription { get; set; }
        } 


        private List<IdPayVerifyPaymentStatusCode> getVerifyStatusCode()
        {
            return new List<IdPayVerifyPaymentStatusCode> {
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 1,
                    Description="پرداخت انجام نشده است",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 2,
                    Description="پرداخت ناموفق بوده است",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 3,
                    Description="خطا رخ داده است",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 4,
                    Description="بلوکه شده",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 5,
                    Description="برگشت به پرداخت کننده",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 6,
                    Description="برگشت خورده سیستمی",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 7,
                    Description="انصراف از پرداخت",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 8,
                    Description="به درگاه پرداخت منتقل شد",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 10,
                    Description="در انتظار تایید پرداخت",
                    IsPaymentPerformed=false
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 100,
                    Description="پرداخت تایید شده است",
                    IsPaymentPerformed=true
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 101,
                    Description="پرداخت قبلا تایید شده است",
                    IsPaymentPerformed=true
                },
                new IdPayVerifyPaymentStatusCode {
                    StatusCode = 200,
                    Description="به دریافت کننده واریز شد",
                    IsPaymentPerformed=true
                },
            };
        }

        private class IdPayVerifyPaymentStatusCode
        {
            public int StatusCode { get; set; }
            public string Description { get; set; }
            public bool IsPaymentPerformed { get; set; }
        }
    }
}
