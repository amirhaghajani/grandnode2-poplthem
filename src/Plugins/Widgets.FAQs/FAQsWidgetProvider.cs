using Grand.Business.Cms.Interfaces;
using Grand.Business.Common.Interfaces.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs
{
    public class FAQsWidgetProvider : IWidgetProvider
    {
        private readonly ITranslationService _translationService;
        private readonly FAQsWidgetSettings _sliderWidgetSettings;

        public FAQsWidgetProvider(ITranslationService translationService, FAQsWidgetSettings sliderWidgetSettings)
        {
            _translationService = translationService;
            _sliderWidgetSettings = sliderWidgetSettings;
        }

        public string ConfigurationUrl => FAQsWidgetDefaults.ConfigurationUrl;

        public string SystemName => FAQsWidgetDefaults.ProviderSystemName;

        public string FriendlyName => _translationService.GetResource(FAQsWidgetDefaults.FriendlyName);

        public int Priority => _sliderWidgetSettings.DisplayOrder;

        public IList<string> LimitedToStores => _sliderWidgetSettings.LimitedToStores;

        public IList<string> LimitedToGroups => _sliderWidgetSettings.LimitedToGroups;

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public async Task<IList<string>> GetWidgetZones()
        {
            return await Task.FromResult(new List<string>
            {
                FAQsWidgetDefaults.WidgetZoneHomePage,
                FAQsWidgetDefaults.WidgetZoneAllFAQsPage,
            });
        }

        public Task<string> GetPublicViewComponentName(string widgetZone)
        {
            return Task.FromResult("WidgetFAQs");
        }

    }
}
