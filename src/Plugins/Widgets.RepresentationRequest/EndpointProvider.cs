using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Widgets.RepresentationRequest
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute("Plugin.Widgets.RepresentationRequest.Create",
                 "Plugins/MyRepresentationRequest/Create",
                 new { controller = "MyRepresentationRequest", action = "Create" }
            );

            endpointRouteBuilder.MapControllerRoute("Plugin.Widgets.RepresentationRequest.Create.small",
                 "new-representation-request",
                 new { controller = "MyRepresentationRequest", action = "Create" }
            );
        }
        public int Priority => 0;

    }
}
