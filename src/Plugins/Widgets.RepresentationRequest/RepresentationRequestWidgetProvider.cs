using Grand.Business.Cms.Interfaces;
using Grand.Business.Common.Interfaces.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.RepresentationRequest
{
    public class RepresentationRequestWidgetProvider: IWidgetProvider
    {
        private readonly ITranslationService _translationService;
        private readonly RepresentationRequestWidgetSettings _requestWidgetSettings;

        public RepresentationRequestWidgetProvider(ITranslationService translationService, RepresentationRequestWidgetSettings requestWidgetSettings)
        {
            _translationService = translationService;
            _requestWidgetSettings = requestWidgetSettings;
        }

        public string ConfigurationUrl => RepresentationRequestWidgetDefaults.ConfigurationUrl;

        public string SystemName => RepresentationRequestWidgetDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(RepresentationRequestWidgetDefaults.FriendlyName);

        public int Priority => _requestWidgetSettings.DisplayOrder;

        public IList<string> LimitedToStores => _requestWidgetSettings.LimitedToStores;

        public IList<string> LimitedToGroups => _requestWidgetSettings.LimitedToGroups;

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public async Task<IList<string>> GetWidgetZones()
        {
            return await Task.FromResult(new List<string>
            {
                RepresentationRequestWidgetDefaults.WidgetZoneRepresentationRequestPage,
            });
        }

        public Task<string> GetPublicViewComponentName(string widgetZone)
        {
            return Task.FromResult("WidgetRepresentationRequest");
        }
    }
}
