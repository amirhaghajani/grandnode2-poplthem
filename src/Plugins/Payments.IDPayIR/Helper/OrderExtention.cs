using Grand.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payments.IDPayIR.Helper
{
    public static class OrderExtention
    {
        public static string MyRoundTotalAmountOfOrder(this Order order)
        {
            var roundedOrderTotal = Math.Round(order.OrderTotal * 1000, 0);
            return roundedOrderTotal.ToString();
        }
    }
}
