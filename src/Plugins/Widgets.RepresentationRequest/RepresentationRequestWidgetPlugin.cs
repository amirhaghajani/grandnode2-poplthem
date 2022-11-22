using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Storage.Interfaces;
using Grand.Domain.Data;
using Grand.Infrastructure.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Domain;

namespace Widgets.RepresentationRequest
{

    public class RepresentationRequestWidgetPlugin : BasePlugin, IPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly IRepository<RepresentationRequestDomain> _representationRequestRepository;
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly IDatabaseContext _databaseContext;

        public RepresentationRequestWidgetPlugin(IPictureService pictureService,
            IRepository<RepresentationRequestDomain> representationRequestRepository,
            ITranslationService translationService,
            ILanguageService languageService,
            IDatabaseContext databaseContext)
        {
            _pictureService = pictureService;
            _representationRequestRepository = representationRequestRepository;
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
            await _databaseContext.CreateIndex(_representationRequestRepository, OrderBuilder<RepresentationRequestDomain>.Create()
                .Ascending(x => x.Id), "RepresentationRequestId_DisplayOrder");



            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.DisplayOrder", "Display order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToGroups", "Limited to groups");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToStores", "Limited to stores");


            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.FriendlyName", "Widget Representation Request");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Added", "Representation Request added");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Addnew", "Add new Representation Request");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.AvailableStores", "Available stores");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.AvailableStores.Hint", "Select stores for which the faqs will be shown.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Backtolist", "Back to list");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Edit", "Edit Representation Request");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Edited", "Representation Request edited");



            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.FullWidth", "Full width");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.FullWidth.hint", "Full width");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Info", "Info");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.LimitedToStores", "Limited to stores");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.LimitedToStores.Hint", "Determines whether the reperesentation request is available only at certain stores.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Link", "URL");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Manage", "Manage Representation Request");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection", "Collection");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection.Hint", "Select the collection where faq should appear.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection.Required", "Collection is required");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand", "Brand");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand.Hint", "Select the brand where faq should appear.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand.Required", "Brand is required");


            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Displayorder", "Display Order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Answer", "Answer");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Question", "Question");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.IsImportantQuestion", "IsImportantQuestion");

            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question", "Question");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question.Hint", "Enter the question of the faq");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question.Required", "Question is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer", "Answer");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer.Hint", "Enter the answer of the faq");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer.Required", "answer is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.IsImportantQuestion", "IsImportantQuestion");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.IsImportantQuestion.Hint", "Specify it should be in homepage");


            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Stores", "Stores");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.DisplayOrder", "Display Order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.DisplayOrder.Hint", "The representation request display order. 1 represents the first item in the list.");


            await base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task Uninstall()
        {

            //clear repository
            await _representationRequestRepository.DeleteAsync(_representationRequestRepository.Table.ToList());

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToGroups");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToStores");


            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.FriendlyName");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Added");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Addnew");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.AvailableStores");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.AvailableStores.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Backtolist");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Edit");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Edited");



            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.FullWidth");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.FullWidth.hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Info");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.LimitedToStores");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.LimitedToStores.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Link");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Link.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Manage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Collection.Required");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Brand.Required");


            //------------------------
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Displayorder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Answer");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Question");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.IsImportantQuestion");

            //------------------------
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Question.Required");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Answer.Required");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.IsImportantQuestion");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.IsImportantQuestion.Hint");


            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Stores");

            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.DisplayOrder.Hint");


            await base.Uninstall();
        }

        public override string ConfigurationUrl()
        {
            return RepresentationRequestWidgetDefaults.ConfigurationUrl;
        }
    }
}
