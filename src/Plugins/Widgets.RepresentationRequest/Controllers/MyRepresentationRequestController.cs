using Grand.Business.Cms.Interfaces;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Storage.Interfaces;
using Grand.Domain.Stores;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Security.Captcha;
using Microsoft.AspNetCore.Http;
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
        private readonly CaptchaSettings _captchaSettings;
        #endregion

        public MyRepresentationRequestController(IPictureService pictureService,
            ITranslationService translationService,
            IRepresentationRequestService requestService,
            ILanguageService languageService,
            ISettingService settingService,
            RepresentationRequestWidgetSettings requestWidgetSettings,
            CaptchaSettings captchaSettings)
        {
            this._pictureService = pictureService;
            this._translationService = translationService;
            this._requestService = requestService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._requestWidgetSettings = requestWidgetSettings;
            this._captchaSettings = captchaSettings;
        }


        public async Task<IActionResult> Create()
        {
            var model = new RepresentationRequestModel();
            //locales
            await AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create( 
            [FromServices] StoreInformationSettings storeInformationSettings,
            [FromServices] IPageService pageService, RepresentationRequestModel model, IFormCollection form, bool captchaValid)
        {
            if (storeInformationSettings.StoreClosed)
            {
                var closestorepage = await pageService.GetPageBySystemName("ContactUs");
                if (closestorepage == null || !closestorepage.AccessibleWhenStoreClosed)
                    return RedirectToRoute("StoreClosed");
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_translationService));
            }

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
