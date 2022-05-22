using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Seo;
using Grand.Infrastructure.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Grand.Web.Themes.Popl.Routing
{
    public class MyRouteTransformer : DynamicRouteValueTransformer
    {
        private readonly ISlugService _slugService;
        private readonly ILanguageService _languageService;
        private readonly AppConfig _config;

        public MyRouteTransformer(
            ISlugService slugService,
            ILanguageService languageService,
            AppConfig config)
        {
            _slugService = slugService;
            _languageService = languageService;
            _config = config;
        }

        protected async ValueTask<string> GetSeName(string entityId, string entityName, string languageId)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(languageId))
            {
                result = await _slugService.GetActiveSlug(entityId, entityName, languageId);
            }
            //set default value if required
            if (string.IsNullOrEmpty(result))
            {
                result = await _slugService.GetActiveSlug(entityId, entityName, "");
            }

            return result;
        }

        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext context, RouteValueDictionary values)
        {
            if (values == null)
                return null;

            var slug = values["SeName"];
            if (slug == null)
                return null;

            var entityUrl = await _slugService.GetBySlugCached(slug.ToString());
            //no URL Entity found
            if (entityUrl == null)
                return null;

            if (slug.ToString().ToLower() != Grand.Web.Themes.Popl.MyConfig.shopAllUrl)
                return null;

            values["controller"] = "MyCatalog";
            values["action"] = "MyCategoryShopAll";
            values["categoryid"] = entityUrl.EntityId;
            values["SeName"] = entityUrl.Slug;

            return values;

        }
    }
}
