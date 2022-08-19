using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Storage.Interfaces;
using Grand.Domain.Data;
using Grand.Infrastructure.Plugins;
using Grand.SharedKernel.Extensions;
using Widgets.FAQs.Domain;

namespace Widgets.FAQs
{
    public class FAQsWidgetPlugin : BasePlugin, IPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly IRepository<FAQ> _faqRepository;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly IDatabaseContext _databaseContext;

        public FAQsWidgetPlugin(IPictureService pictureService,
            IRepository<FAQ> faqRepository,
            ITranslationService translationService,
            ILanguageService languageService,
            IDatabaseContext databaseContext)
        {
            _pictureService = pictureService;
            _faqRepository = faqRepository;
            _translationService = translationService;
            _languageService = languageService;
            _databaseContext = databaseContext;
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task Install()
        {
            //Create index
            await _databaseContext.CreateIndex(_faqRepository, OrderBuilder<FAQ>.Create()
                .Ascending(x => x.IsImportantQuestion).Ascending(x => x.DisplayOrder), "FAQsIsImportantQuestion_DisplayOrder");



            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.DisplayOrder", "Display order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.LimitedToGroups", "Limited to groups");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.LimitedToStores", "Limited to stores");


            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.FriendlyName", "Widget FAQs");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Added", "FAQs added");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Addnew", "Add new faq");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.AvailableStores", "Available stores");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.AvailableStores.Hint", "Select stores for which the faqs will be shown.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Backtolist", "Back to list");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Edit", "Edit FAQ");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Edited", "FAQ edited");



            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.FullWidth", "Full width");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.FullWidth.hint", "Full width");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Info", "Info");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.LimitedToStores", "Limited to stores");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.LimitedToStores.Hint", "Determines whether the faq is available only at certain stores.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Link", "URL");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Manage", "Manage FAQ");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Collection", "Collection");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Collection.Hint", "Select the collection where faq should appear.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Collection.Required", "Collection is required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Brand", "Brand");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Brand.Hint", "Select the brand where faq should appear.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Brand.Required", "Brand is required");


            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.Displayorder", "Display Order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.Answer", "Answer");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.Question", "Question");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Fields.IsImportantQuestion", "IsImportantQuestion");

            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Question", "Question");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Question.Hint", "Enter the question of the faq");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Question.Required", "Question is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Answer", "Answer");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Answer.Hint", "Enter the answer of the faq");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Answer.Required", "answer is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.IsImportantQuestion", "IsImportantQuestion");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.IsImportantQuestion.Hint", "Specify it should be in homepage");


            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.Stores", "Stores");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.DisplayOrder", "Display Order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.FAQs.DisplayOrder.Hint", "The faq display order. 1 represents the first item in the list.");


            await base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task Uninstall()
        {

            //clear repository
            await _faqRepository.DeleteAsync(_faqRepository.Table.ToList());

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.LimitedToGroups");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.LimitedToStores");


            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.FriendlyName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Added");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Addnew");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.AvailableStores");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.AvailableStores.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Backtolist");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Edit");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Edited");



            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.FullWidth");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.FullWidth.hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Info");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.LimitedToStores");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.LimitedToStores.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Link");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Link.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Manage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Collection");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Collection.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Collection.Required");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Brand");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Brand.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Brand.Required");


            //------------------------
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.Displayorder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.Answer");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.Question");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Fields.IsImportantQuestion");

            //------------------------
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Question");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Question.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Question.Required");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Answer");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Answer.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Answer.Required");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.IsImportantQuestion");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.IsImportantQuestion.Hint");


            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.Stores");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.FAQs.DisplayOrder.Hint");


            await base.Uninstall();
        }

        public override string ConfigurationUrl()
        {
            return FAQsWidgetDefaults.ConfigurationUrl;
        }
    }
}

