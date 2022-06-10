using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;

namespace Payments.IDPayIR.Models
{
    public class ConfigurationModel : BaseModel
    {
        public string StoreScope { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.BusinessEmail")]
        public string BusinessEmail { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.ApiToken")]
        public string ApiToken { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.PDTToken")]
        //public string PdtToken { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.PDTValidateOrderTotal")]
        //public bool PdtValidateOrderTotal { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.AdditionalFee")]
        //public double AdditionalFee { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.AdditionalFeePercentage")]
        //public bool AdditionalFeePercentage { get; set; }

        //[GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.PassProductNamesAndTotals")]
        //public bool PassProductNamesAndTotals { get; set; }

        [GrandResourceDisplayName("Plugins.Payments.IDPayIR.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }



    }
}