using Grand.Domain.Localization;
using Grand.Domain.Stores;
using Grand.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs.Domain
{
    public partial class FAQ : BaseEntity, ITranslationEntity, IStoreLinkEntity
    {
        public FAQ()
        {
            Stores = new List<string>();
            Locales = new List<TranslationEntity>();
        }

        public string FAQId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public bool IsImportantQuestion { get; set; }
        public int DisplayOrder { get; set; }



        //ITranslationEntity-------------------------------------------------
        public IList<TranslationEntity> Locales { get; set; }
        //-----------------------------------------------------------------


        //IStoreLinkEntity -------------------------------------------------
        public bool LimitedToStores { get; set; }
        public IList<string> Stores { get; set; }
        //-----------------------------------------------------------------
    }
}
