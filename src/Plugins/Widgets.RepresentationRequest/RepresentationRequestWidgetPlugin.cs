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
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.DisplayOrder", "Display order");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToGroups", "Limited to groups");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToStores", "Limited to stores");


            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.inputform.head1", "head1");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.inputform.head2", "head2");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.inputform.head3", "head3");
            //------------------------
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FullName", "Full name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FullName.Hint", "Enter your full name");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FullName.Required", "fullname is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Age", "Age");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Age.Hint", "Enter your age");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Age.Required", "fullname is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LevelOfEducation", "Level Of education");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LevelOfEducation.Hint", "Enter your level of education");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LevelOfEducation.Required", "level of education is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FieldOfStudy", "FieldOfStudy");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FieldOfStudy.label", "Field of study");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FieldOfStudy.Hint", "Enter your field of study");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.FieldOfStudy.Required", "Field of study of education is required");


            //
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Address", "Address");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Address.Hint", "Enter your address");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Address.Required", "address is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Job", "Job");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Job.Hint", "Enter your job");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.Job.Required", "job is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.JobExperience", "Job Experience (Year)");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.JobExperience.Hint", "Enter your job experience (year)");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.JobExperience.Required", "job experience is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkAddress", "Work Address");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkAddress.Hint", "Enter your work address");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkAddress.Required", "work address is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkTel", "Work Tel");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkTel.Hint", "Enter your work tel");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WorkTel.Required", "work tel is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.BusinessWebsite", "Business Website");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.BusinessWebsite.Hint", "Enter your business website");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.BusinessWebsite.Required", "business website is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.InstagramChannel", "Instagram Channel");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.InstagramChannel.Hint", "Enter your instagram channel");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.InstagramChannel.Required", "instagram channel is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WhoDidYouGetToKnowZAP", "Who did you get to know ZAP");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WhoDidYouGetToKnowZAP.Hint", "Enter who did you get to know ZAP");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WhoDidYouGetToKnowZAP.Required", "Who did you get to know ZAP is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.StrengthAndWeakness", "Strength and weakness of ZAP");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.StrengthAndWeakness.Hint", "Enter Strength and weakness of ZAP");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.StrengthAndWeakness.Required", "Strength and weakness of ZAP is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.EstimateOfSell", "Estimate of sell");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.EstimateOfSell.Hint", "Enter your estimate of sell");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.EstimateOfSell.Required", "Estimate of sell is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.SellPromotionalProgram", "Sell promotional program");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.SellPromotionalProgram.Hint", "Enter your sell promotional program");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.SellPromotionalProgram.Required", "Sell promotional program is required");

            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WantedCities", "Wanted cities");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WantedCities.Hint", "Enter your wanted cities");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.WantedCities.Required", "Wanted cities is required");



            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.Stores", "Stores");


            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Widgets.RepresentationRequest.SendButton", "Send Form");


            



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
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.DisplayOrder");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToGroups");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Fields.LimitedToStores");

            //------------------------



            await this.DeletePluginTranslationResource(_translationService, _languageService, "Widgets.RepresentationRequest.Stores");



            await base.Uninstall();
        }

        public override string ConfigurationUrl()
        {
            return RepresentationRequestWidgetDefaults.ConfigurationUrl;
        }
    }
}
