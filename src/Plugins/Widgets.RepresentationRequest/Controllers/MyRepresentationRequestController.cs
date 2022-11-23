using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Storage.Interfaces;
using Grand.Web.Common.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Models;
using Widgets.RepresentationRequest.Services;

namespace Widgets.RepresentationRequest.Controllers
{
    public class MyRepresentationRequestController: BasePluginController
    {
        #region fields
        private readonly IPictureService _pictureService;
        private readonly ITranslationService _translationService;
        private readonly IRepresentationRequestService _requestService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly RepresentationRequestWidgetSettings _requestWidgetSettings;
        #endregion

        public MyRepresentationRequestController(IPictureService pictureService,
            ITranslationService translationService,
            IRepresentationRequestService requestService,
            ILanguageService languageService,
            ISettingService settingService,
            RepresentationRequestWidgetSettings requestWidgetSettings)
        {
            this._pictureService = pictureService;
            this._translationService = translationService;
            this._requestService = requestService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._requestWidgetSettings = requestWidgetSettings;
        }


        [HttpPost]
        public async Task<IActionResult> Create(RepresentationRequestModel model)
        {
            if (ModelState.IsValid)
            {
                var request = model.ToEntity();
                request.Locales = model.Locales.ToLocalizedProperty();

                await _requestService.InsertRequest(request);

                Success(_translationService.GetResource("Widgets.RepresentationRequest.Added"));
                return RedirectToAction("CreateSuccessfully", new { id = request.Id });

            }
            return View(model);
        }

        public IActionResult CreateSuccessfully()
        {
            return View();
        }
    }
}
