using Grand.Domain.Data;
using Grand.Infrastructure.Configuration;
using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;


namespace Grand.Web.Themes.Popl.Endpoints
{
    public partial class MyEndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = "";
            if (DataSettingsManager.DatabaseIsInstalled())
            {
                var config = endpointRouteBuilder.ServiceProvider.GetRequiredService<AppConfig>();
                if (config.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    pattern = $"{{language:lang={config.SeoFriendlyUrlsDefaultCode}}}/";
                }
            }

            RegisterAddToCartRoute(endpointRouteBuilder, pattern);
        }

        public int Priority => 1;


        private void RegisterAddToCartRoute(IEndpointRouteBuilder endpointRouteBuilder, string pattern)
        {

            //add product to cart (without any attributes and options). used on catalog pages.
            endpointRouteBuilder.MapControllerRoute("MyAddProductCatalog",
                            pattern + "myaddproducttocart/catalog/{productId?}/{shoppingCartTypeId?}",
                            new { controller = "MyActionCart", action = "AddProductCatalog" },
                            new { productId = @"\w+", shoppingCartTypeId = @"\d+" },
                            new[] { "Grand.Web.Controllers" });
        }
    }
}
