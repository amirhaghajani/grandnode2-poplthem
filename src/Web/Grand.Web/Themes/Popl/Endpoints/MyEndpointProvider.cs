using Grand.Domain.Data;
using Grand.Infrastructure.Configuration;
using Grand.Infrastructure.Endpoints;
using Grand.Web.Themes.Popl.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Grand.Web.Themes.Popl.Endpoints
{
    public partial class MyEndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            if (!DataSettingsManager.DatabaseIsInstalled())
                return;

            var pattern = "{**SeName}";
            var config = endpointRouteBuilder.ServiceProvider.GetRequiredService<AppConfig>();
            if (config.SeoFriendlyUrlsForLanguagesEnabled)
            {
                pattern = $"{{language:lang={config.SeoFriendlyUrlsDefaultCode}}}/{{**SeName}}";
            }

            endpointRouteBuilder.MapDynamicControllerRoute<MyRouteTransformer>(pattern);
        }

        public int Priority
        {
            get
            {
                //it should be the last route
                //we do not set it to -int.MaxValue so it could be overridden (if required)
                return -100;
            }
        }
    }
}
