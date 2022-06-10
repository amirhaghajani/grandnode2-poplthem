using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.IDPayIR
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Payment callback
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.IdPayIR.PaymentHandler",
                 "Plugins/PaymentIDPayIR/PaymentHandler",
                 new { controller = "PaymentIDPayIR", action = "PaymentHandler" }
            );
            
        }
        public int Priority => 0;

    }
}
