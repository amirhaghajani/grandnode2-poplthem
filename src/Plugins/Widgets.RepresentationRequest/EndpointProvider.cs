using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Widgets.RepresentationRequest
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //PDT
            endpointRouteBuilder.MapControllerRoute("Plugin.Widgets.RepresentationRequest.Create",
                 "Plugins/MyRepresentationRequest/Create",
                 new { controller = "MyRepresentationRequest", action = "Create" }
            );
            
        }
        public int Priority => 0;

    }
}
