using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Payments.BahamtaIR.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.UseSandbox")]
        //public bool UseSandbox { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.BusinessEmail")]
        public string BusinessEmail { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.ApiToken")]
        public string ApiToken { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.PDTToken")]
        //public string PdtToken { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.PDTValidateOrderTotal")]
        //public bool PdtValidateOrderTotal { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.AdditionalFee")]
        //public double AdditionalFee { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.AdditionalFeePercentage")]
        //public bool AdditionalFeePercentage { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.PassProductNamesAndTotals")]
        //public bool PassProductNamesAndTotals { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.BahamtaIR.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }



    }
}