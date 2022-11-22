using Grand.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.RepresentationRequest.Models
{
    public class RepresentationRequestInListModel: BaseModel
    {
        public string Fullname { get; set; }

        public int Age { get; set; }

        /// <summary>
        /// سطح تحصیلات
        /// </summary>
        public string LevelOfEducation { get; set; }

        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        public string FieldOfStudy { get; set; }


        public string Address { get; set; }

        /// <summary>
        /// شهرهایی که درخواست نمایندگی دارید
        /// </summary>
        public string WantedCities { get; set; }
    }
}
