using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs.Models
{
    public class PublicInfoModel
    {
        public PublicInfoModel()
        {
            this.FAQsList = new List<PublicFAQ>();
        }

        public IList<PublicFAQ> FAQsList { get; set; }
        public class PublicFAQ
        {
            public string Question { get; set; }
            public string Answer { get; set; }
        }
    }
}
