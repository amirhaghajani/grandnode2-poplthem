using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.BahamtaIR
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Payment callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.BahamtaIR.PaymentHandler",
                 "Plugins/PaymentBahamtaIR/PaymentHandler",
                 new { controller = "PaymentBahamtaIR", action = "PaymentHandler" }
            );
            
        }
        public int Priority => 0;

    }
}
