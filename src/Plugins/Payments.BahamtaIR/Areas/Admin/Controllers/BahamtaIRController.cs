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
using Payments.BahamtaIR.Models;

namespace Payments.BahamtaIR.Controllers
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [PermissionAuthorize(PermissionSystemName.PaymentMethods)]
    public class BahamtaIRController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly ITranslationService _translationService;
        private readonly IPermissionService _permissionService;

        public BahamtaIRController(IWorkContext workContext,
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
            var BahamtaIRPaymentSettings = _settingService.LoadSetting<BahamtaIRPaymentSettings>(storeScope);

            var model = new ConfigurationModel();
            //model.UseSandbox = BahamtaIRPaymentSettings.UseSandbox;
            model.BusinessEmail = BahamtaIRPaymentSettings.BusinessEmail;
            model.ApiToken = BahamtaIRPaymentSettings.ApiToken;
            //model.PdtToken = BahamtaIRPaymentSettings.PdtToken;
            //model.PdtValidateOrderTotal = BahamtaIRPaymentSettings.PdtValidateOrderTotal;
            //model.AdditionalFee = BahamtaIRPaymentSettings.AdditionalFee;
            //model.AdditionalFeePercentage = BahamtaIRPaymentSettings.AdditionalFeePercentage;
            //model.PassProductNamesAndTotals = BahamtaIRPaymentSettings.PassProductNamesAndTotals;
            model.DisplayOrder = BahamtaIRPaymentSettings.DisplayOrder;

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
            var BahamtaIRPaymentSettings = _settingService.LoadSetting<BahamtaIRPaymentSettings>(storeScope);

            //save settings
            //BahamtaIRPaymentSettings.UseSandbox = model.UseSandbox;
            BahamtaIRPaymentSettings.BusinessEmail = model.BusinessEmail;
            BahamtaIRPaymentSettings.ApiToken = model.ApiToken;
            //BahamtaIRPaymentSettings.PdtToken = model.PdtToken;
            //BahamtaIRPaymentSettings.PdtValidateOrderTotal = model.PdtValidateOrderTotal;
            //BahamtaIRPaymentSettings.AdditionalFee = model.AdditionalFee;
            //BahamtaIRPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            //BahamtaIRPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            BahamtaIRPaymentSettings.DisplayOrder = model.DisplayOrder;

            await _settingService.SaveSetting(BahamtaIRPaymentSettings, storeScope);

            //now clear settings cache
            await _settingService.ClearCache();

            Success(_translationService.GetResource("Admin.Plugins.Saved"));

            return await Configure();
        }

    }
}