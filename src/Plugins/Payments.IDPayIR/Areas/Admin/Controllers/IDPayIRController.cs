using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Security;
using Grand.Business.Common.Interfaces.Stores;
using Grand.Business.Common.Services.Security;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Infrastructure;
using Grand.Web.Common.Controllers;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.IDPayIR.Models;

namespace Payments.IDPayIR.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.PaymentMethods)]
    public class IDPayIRController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly IPermissionService _permissionService;

        public IDPayIRController(IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            ITranslationService translationService,
            IPermissionService permissionService)
        {
            _workContext = workContext;
            _storeService = storeService;
            _settingService = settingService;
            _translationService = translationService;
            _permissionService = permissionService;
        }

        protected virtual async Task<string> GetActiveStore(IStoreService storeService, IWorkContext workContext)
        {
            var stores = await storeService.GetAllStores();
            if (stores.Count < 2)
                return stores.FirstOrDefault().Id;

            var storeId = workContext.CurrentCustomer.GetUserFieldFromEntity<string>(SystemCustomerFieldNames.AdminAreaStoreScopeConfiguration);
            var store = await storeService.GetStoreById(storeId);

            return store != null ? store.Id : "";
        }

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await GetActiveStore(_storeService, _workContext);
            var IDPayIRPaymentSettings = _settingService.LoadSetting<IDPayIRPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            model.UseSandbox = IDPayIRPaymentSettings.UseSandbox;
            model.BusinessEmail = IDPayIRPaymentSettings.BusinessEmail;
            model.ApiToken = IDPayIRPaymentSettings.ApiToken;
            //model.PdtToken = IDPayIRPaymentSettings.PdtToken;
            //model.PdtValidateOrderTotal = IDPayIRPaymentSettings.PdtValidateOrderTotal;
            //model.AdditionalFee = IDPayIRPaymentSettings.AdditionalFee;
            //model.AdditionalFeePercentage = IDPayIRPaymentSettings.AdditionalFeePercentage;
            //model.PassProductNamesAndTotals = IDPayIRPaymentSettings.PassProductNamesAndTotals;
            model.DisplayOrder = IDPayIRPaymentSettings.DisplayOrder;

            model.StoreScope = storeScope;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.Authorize(StandardPermission.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await this.GetActiveStore(_storeService, _workContext);
            var IDPayIRPaymentSettings = _settingService.LoadSetting<IDPayIRPaymentSettings>(storeScope);

            //save settings
            IDPayIRPaymentSettings.UseSandbox = model.UseSandbox;
            IDPayIRPaymentSettings.BusinessEmail = model.BusinessEmail;
            IDPayIRPaymentSettings.ApiToken = model.ApiToken;
            //IDPayIRPaymentSettings.PdtToken = model.PdtToken;
            //IDPayIRPaymentSettings.PdtValidateOrderTotal = model.PdtValidateOrderTotal;
            //IDPayIRPaymentSettings.AdditionalFee = model.AdditionalFee;
            //IDPayIRPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            //IDPayIRPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            IDPayIRPaymentSettings.DisplayOrder = model.DisplayOrder;

            await _settingService.SaveSetting(IDPayIRPaymentSettings, storeScope);

            //now clear settings cache
            await _settingService.ClearCache();

            Success(_translationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

    }
}