using Newtonsoft.Json;
using Payments.IDPayIR.Services;

namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new IdPayIrHttpClient(new HttpClient());
            var request = new IdPayDataNeedForCreatePayment {
                order_id = "104",
                amount = "900",
                name = "قاسم رادمان",
                phone = "09382198592",
                mail = "my@site.com",
                desc = "توضیحات پرداخت کننده",
                callback = "https://example.com/callback",
            };
            var answer = await client.CreateIdPayPayment("6a7f99eb-7c20-4412-a972-6dfb7cd253a4", request
                , true);

            var answer2 = await client.VerifyPayment("6a7f99eb-7c20-4412-a972-6dfb7cd253a4",answer.PaymentTransactionId, request.order_id,true );
            

            Console.ReadLine();
        }

        
    }
}