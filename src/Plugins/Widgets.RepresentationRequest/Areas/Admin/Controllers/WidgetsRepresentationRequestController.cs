using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Services.Security;
using Grand.Business.Storage.Interfaces;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.DataSource;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Models;
using Widgets.RepresentationRequest.Services;

namespace Widgets.RepresentationRequest.Areas.Admin.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.Widgets)]
    public class WidgetsRepresentationRequestController: BasePluginController
    {
        #region fields
        private readonly IPictureService _pictureService;
        private readonly ITranslationService _translationService;
        private readonly IRepresentationRequestService _requestService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;
        private readonly RepresentationRequestWidgetSettings _requestWidgetSettings;
        #endregion


        public WidgetsRepresentationRequestController(IPictureService pictureService,
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


        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.DisplayOrder = _requestWidgetSettings.DisplayOrder;
            model.CustomerGroups = _requestWidgetSettings.LimitedToGroups?.ToArray();
            model.Stores = _requestWidgetSettings.LimitedToStores?.ToArray();
            return View(model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            _requestWidgetSettings.DisplayOrder = model.DisplayOrder;
            _requestWidgetSettings.LimitedToGroups = model.CustomerGroups == null ? new List<string>() : model.CustomerGroups.ToList();
            _requestWidgetSettings.LimitedToStores = model.Stores == null ? new List<string>() : model.Stores.ToList();
            _settingService.SaveSetting(_requestWidgetSettings);
            return Json("Ok");
        }


        [HttpPost]
        public async Task<IActionResult> List(DataSourceRequest command)
        {
            var requests = await _requestService.GetRequests();

            var items = new List<RepresentationRequestInListModel>();
            foreach (var x in requests)
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
                Total = requests.Count
            };
            return Json(gridModel);
        }

        public async Task<IActionResult> Create()
        {
            var model = new RepresentationRequestModel();
            //locales
            await AddLocales(_languageService, model.Locales);

            return View(model);
        }

        [HttpPost, ArgumentNameFilter(KeyName = "save-continue", Argument = "continueEditing")]
        public async Task<IActionResult> Create(RepresentationRequestModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var request = model.ToEntity();
                request.Locales = model.Locales.ToLocalizedProperty();

                await _requestService.InsertRequest(request);

                Success(_translationService.GetResource("Widgets.Slider.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = request.Id }) : RedirectToAction("Configure");

            }
            return View(model);
        }
        public async Task<IActionResult> Edit(string id)
        {
            var slide = await _requestService.GetById(id);
            if (slide == null)
                return RedirectToAction("Configure");

            var model = slide.ToModel();

            //locales
            await AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                //locale.Question = slide.GetTranslation(x => x.Question, languageId, false);
                //locale.Answer = slide.GetTranslation(x => x.Answer, languageId, false);
            });

            return View(model);
        }

        [HttpPost, ArgumentNameFilter(KeyName = "save-continue", Argument = "continueEditing")]
        public async Task<IActionResult> Edit(RepresentationRequestModel model, bool continueEditing)
        {
            var request = await _requestService.GetById(model.Id);
            if (request == null)
                return RedirectToAction("Configure");

            if (ModelState.IsValid)
            {
                request = model.ToEntity();
                request.Locales = model.Locales.ToLocalizedProperty();
                await _requestService.UpdateRequest(request);
                Success(_translationService.GetResource("Widgets.RepresentationRequest.Edited"));
                return continueEditing ? RedirectToAction("Edit", new { id = request.Id }) : RedirectToAction("Configure");

            }
            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var request = await _requestService.GetById(id);
            if (request == null)
                return Json(new DataSourceResult { Errors = "This Representation Request not exists" });

            await _requestService.DeleteRequest(request);

            return new JsonResult("");
        }
    }
}
