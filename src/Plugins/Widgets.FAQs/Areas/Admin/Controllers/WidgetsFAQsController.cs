using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Services.Security;
using Grand.Business.Storage.Interfaces;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.DataSource;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using MassTransit.Courier;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.FAQs.Models;
using Widgets.FAQs.Services;

namespace Widgets.FAQs.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.Widgets)]
    public class WidgetsFAQsController: BasePluginController
    {
        #region fields
        private readonly IPictureService _pictureService;
        private readonly ITranslationService _translationService;
        private readonly IFAQsService _faqService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly FAQsWidgetSettings _faqWidgetSettings;
        #endregion


        public WidgetsFAQsController(IPictureService pictureService,
            ITranslationService translationService,
            IFAQsService faqService,
            ILanguageService languageService,
            ISettingService settingService,
            FAQsWidgetSettings faqWidgetSettings)
        {
            this._pictureService = pictureService;
            this._translationService = translationService;
            this._faqService = faqService;
            this._languageService = languageService;
            this._settingService = settingService;
            this._faqWidgetSettings = faqWidgetSettings;
        }


        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DisplayOrder = _faqWidgetSettings.DisplayOrder;
            model.CustomerGroups = _faqWidgetSettings.LimitedToGroups?.ToArray();
            model.Stores = _faqWidgetSettings.LimitedToStores?.ToArray();
            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            _faqWidgetSettings.DisplayOrder = model.DisplayOrder;
            _faqWidgetSettings.LimitedToGroups = model.CustomerGroups == null ? new List<string>() : model.CustomerGroups.ToList();
            _faqWidgetSettings.LimitedToStores = model.Stores == null ? new List<string>() : model.Stores.ToList();
            _settingService.SaveSetting(_faqWidgetSettings);
            return Json("Ok");
        }


        [HttpPost]
        public async Task<IActionResult> List(DataSourceRequest command)
        {
            var faqs = await _faqService.GetFaqs();

            var items = new List<FaqInListModel>();
            foreach (var x in faqs)
            {
                var model = x.ToListModel();
                //var picture = await _pictureService.GetPictureById(x.PictureId);
                //if (picture != null)
                //{
                //    model.PictureUrl = await _pictureService.GetPictureUrl(picture, 150);
                //}
                items.Add(model);
            }
            var gridModel = new DataSourceResult {
                Data = items,
                Total = faqs.Count
            };
            return Json(gridModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new FaqModel();
            //locales
            await AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost, ArgumentNameFilter(KeyName = "save-continue", Argument = "continueEditing")]
        public async Task<IActionResult> Create(FaqModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var pictureSlider = model.ToEntity();
                pictureSlider.Locales = model.Locales.ToLocalizedProperty();

                await _faqService.InsertFaq(pictureSlider);

                Success(_translationService.GetResource("Widgets.Slider.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = pictureSlider.Id }) : RedirectToAction("Configure");

            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var slide = await _faqService.GetById(id);
            if (slide == null)
                return RedirectToAction("Configure");

            var model = slide.ToModel();

            //locales
            await AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Question = slide.GetTranslation(x => x.Question, languageId, false);
                locale.Answer = slide.GetTranslation(x => x.Answer, languageId, false);
            });

            return View(model);
        }

        [HttpPost, ArgumentNameFilter(KeyName = "save-continue", Argument = "continueEditing")]
        public async Task<IActionResult> Edit(FaqModel model, bool continueEditing)
        {
            var faq = await _faqService.GetById(model.Id);
            if (faq == null)
                return RedirectToAction("Configure");

            if (ModelState.IsValid)
            {
                faq = model.ToEntity();
                faq.Locales = model.Locales.ToLocalizedProperty();
                await _faqService.UpdateFaq(faq);
                Success(_translationService.GetResource("Widgets.FAQs.Edited"));
                return continueEditing ? RedirectToAction("Edit", new { id = faq.Id }) : RedirectToAction("Configure");

            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var faq = await _faqService.GetById(id);
            if (faq == null)
                return Json(new DataSourceResult { Errors = "This FAQ not exists" });

            await _faqService.DeleteFaq(faq);

            return new JsonResult("");
        }
    }
}
