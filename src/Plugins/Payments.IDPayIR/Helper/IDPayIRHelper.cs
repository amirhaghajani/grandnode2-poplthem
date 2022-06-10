using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.IDPayIR.Helper
{
    internal class IDPayIRHelper
    {
        public static string IdPayCreatePaymentURL => "https://api.idpay.ir/v1.1/payment";
        public static string IdPayVerifyPaymentURL => "https://api.idpay.ir/v1.1/payment/verify";
    }
}
