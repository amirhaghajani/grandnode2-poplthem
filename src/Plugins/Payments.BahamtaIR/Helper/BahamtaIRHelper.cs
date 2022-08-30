using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.BahamtaIR.Helper
{
    internal class BahamtaIRHelper
    {
        public static string BahamtaCreatePaymentURL => "https://webpay.bahamta.com/api/create_request"; //get
        public static string BahamtaVerifyPaymentURL => "https://webpay.bahamta.com/api/confirm_payment"; //get
    }
}
