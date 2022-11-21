using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Grand.Web.Common.Link;
using Grand.Web.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs.Models
{
    public class FaqModel : BaseEntityModel, ILocalizedModel<FaqLocalizedModel>, IStoreLinkModel
    {
        public FaqModel()
        {
            Locales = new List<FaqLocalizedModel>();
        }


        [GrandResourceDisplayName("Widgets.FAQs.Question")]
        public string Question { get; set; }

        [GrandResourceDisplayName("Widgets.FAQs.Answer")]
        public string Answer { get; set; }

        [GrandResourceDisplayName("Widgets.FAQs.IsImportantQuestion")]
        public bool IsImportantQuestion { get; set; }

        [GrandResourceDisplayName("Widgets.FAQs.DisplayOrder")]
        public int DisplayOrder { get; set; }



        //ILocalizedModel------------------
        public IList<FaqLocalizedModel> Locales { get; set; }
        //---------------------------------


        //IStoreLinkModel ----------
        //Store acl
        [GrandResourceDisplayName("Widgets.FAQs.LimitedToStores")]
        [UIHint("Stores")]
        public string[] Stores { get; set; }
        //-------------------------
    }


    public partial class FaqLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        [GrandResourceDisplayName("Widgets.FAQs.Question")]
        public string Question { get; set; }

        [GrandResourceDisplayName("Widgets.FAQs.Answer")]
        public string Answer { get; set; }

    }
}
