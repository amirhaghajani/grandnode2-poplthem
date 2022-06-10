using Grand.Business.Common.Extensions;
using Grand.Business.Common.Interfaces.Configuration;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Plugins;

namespace Payments.IDPayIR
{
    /// <summary>
    /// IDPayIR payment processor
    /// </summary>
    public class IDPayIRPaymentPlugin : BasePlugin, IPlugin
    {
        #region Fields

        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public IDPayIRPaymentPlugin(
            ITranslationService translationService,
            ILanguageService languageService,
            ISettingService settingService)
        {
            _translationService = translationService;
            _languageService = languageService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string ConfigurationUrl()
        {
            return IDPayIRPaymentDefaults.ConfigurationUrl;
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task Install()
        {
            //settings
            await _settingService.SaveSetting(new IDPayIRPaymentSettings
            {
                UseSandbox = true
            });

            //locales
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Payments.IDPayIR.FriendlyName", "ID Pay.ir");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.ApiToken", "Id Pay servcie API Token");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.ApiToken.Hint", "Api code for your webservice in Id pay account");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFee", "Additional fee");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFee.Hint", "Enter additional fee to charge your customers.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.BusinessEmail", "Business Email");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.BusinessEmail.Hint", "Specify your PayPal business email.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PassProductNamesAndTotals", "Pass product names and order totals to PayPal");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to PayPal.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PDTToken", "PDT Identity Token");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PDTValidateOrderTotal", "PDT. Validate order total");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PDTValidateOrderTotal.Hint", "Check if PDT handler should validate order totals.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.RedirectionTip", "You will be redirected to IdPay.ir site to complete the order.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.UseSandbox", "Use Sandbox");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.UseSandbox.Hint", "Check to enable Sandbox (testing environment).");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Instructions", "<p><b>If you're using this gateway ensure that your primary store currency is supported by PayPal.</b><br /><br />To use PDT, you must activate PDT and Auto Return in your PayPal account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to PayPal. Follow these steps to configure your account for PDT:<br /><br />1. Log in to your PayPal account (click <a href=\"https://www.paypal.com/us/webapps/mpp/referral/paypal-business-account2?partner_id=9JJPJNNPQ7PZ8\" target=\"_blank\">here</a> to create your account).<br />2. Click the Profile subtab.<br />3. Click Website Payment Preferences in the Seller Preferences column.<br />4. Under Auto Return for Website Payments, click the On radio button.<br />5. For the Return URL, enter the URL on your site that will receive the transaction ID posted by PayPal after a customer payment ({0}).<br />6. Under Payment Data Transfer, click the On radio button.<br />7. Click Save.<br />8. Click Website Payment Preferences in the Seller Preferences column.<br />9. Scroll down to the Payment Data Transfer section of the page to view your PDT identity token.<br /><br /></p>");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.PaymentMethodDescription", "You will be redirected to IdPay.ir site to complete the payment");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.RoundingWarning", "It looks like you have \"ShoppingCartSettings.RoundPricesDuringCalculation\" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal only rounds to two doubles.");
            await this.AddOrUpdatePluginTranslateResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.DisplayOrder", "Display order");


            await base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task Uninstall()
        {
            //settings
            await _settingService.DeleteSetting<IDPayIRPaymentSettings>();

            //locales
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.ApiToken");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.ApiToken.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFee");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFee.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFeePercentage");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.AdditionalFeePercentage.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.BusinessEmail");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.BusinessEmail.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PassProductNamesAndTotals");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PassProductNamesAndTotals.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PDTToken");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.PDTToken.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.RedirectionTip");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.UseSandbox");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Fields.UseSandbox.Hint");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.Instructions");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.PaymentMethodDescription");
            await this.DeletePluginTranslationResource(_translationService, _languageService, "Plugins.Payments.IDPayIR.RoundingWarning");

            await base.Uninstall();
        }

        #endregion

    }
}