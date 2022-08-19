using Grand.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.FAQs.Models
{
    public class FaqInListModel:BaseModel
    {
        public string Id { get; set; }
        public string Answer { get; set; }
        public string Question { get; set; }
        public string DisplayOrder { get; set; }
        public bool IsImportantQuestion { get; set; }
    }
}
