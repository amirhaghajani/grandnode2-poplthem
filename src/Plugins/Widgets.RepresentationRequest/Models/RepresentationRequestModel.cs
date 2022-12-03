﻿using Grand.Infrastructure.ModelBinding;
using Grand.Infrastructure.Models;
using Grand.Web.Common.Link;
using Grand.Web.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Widgets.RepresentationRequest.Models
{
    public class RepresentationRequestModel: BaseEntityModel, ILocalizedModel<RepresentationRequestLocalizedModel>, IStoreLinkModel
    {
        public RepresentationRequestModel()
        {
            Locales = new List<RepresentationRequestLocalizedModel>();
        }


        //-----------------------
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.FullName")]
        public string FullName { get; set; }

        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.Age")]
        public int Age { get; set; }

        /// <summary>
        /// سطح تحصیلات
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.LevelOfEducation")]
        public string LevelOfEducation { get; set; }

        /// <summary>
        /// رشته تحصیلی
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.FieldOfStudy")]
        public string FieldOfStudy { get; set; }


        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.Address")]
        public string Address { get; set; }


        /// <summary>
        /// شغل
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.Job")]
        public string Job { get; set; }


        /// <summary>
        /// سابقه فعالیت در شغل چند سال است
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.JobExperience")]
        public int JobExperience { get; set; }

        /// <summary>
        /// آدرس محل کار
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.WorkAddress")]
        public string WorkAddress { get; set; }

        /// <summary>
        /// تلفن محل کار
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.WorkTel")]
        public string WorkTel { get; set; }


        /// <summary>
        /// وب سایت کسب و کار
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.BusinessWebsite")]
        public string BusinessWebsite { get; set; }



        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.InstagramChannel")]
        public string InstagramChannel { get; set; }


        /// <summary>
        /// نحوه آشنایی با زپ
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.WhoDidYouGetToKnowZAP")]
        public string WhoDidYouGetToKnowZAP { get; set; }


        /// <summary>
        /// نقاط ضعف و قوت زپ چه بوده
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.StrengthAndWeakness")]
        public string StrengthAndWeakness { get; set; }


        /// <summary>
        /// پیش بینی تعداد فروش در سال
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.EstimateOfSell")]
        public int EstimateOfSell { get; set; }


        /// <summary>
        /// برنامه تبلیغاتی برای فروش
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.SellPromotionalProgram")]
        public string SellPromotionalProgram { get; set; }


        /// <summary>
        /// شهرهایی که درخواست نمایندگی دارید
        /// </summary>
        [GrandResourceDisplayName("Widgets.RepresentationRequest.Fields.WantedCities")]
        public string WantedCities { get; set; }



        //ILocalizedModel------------------
        public IList<RepresentationRequestLocalizedModel> Locales { get; set; }
        //---------------------------------


        //IStoreLinkModel ----------
        //Store acl
        [GrandResourceDisplayName("Widgets.RepresentationRequest.LimitedToStores")]
        [UIHint("Stores")]
        public string[] Stores { get; set; }
        //-------------------------
    }

    public partial class RepresentationRequestLocalizedModel : ILocalizedModelLocal
    {
        public string LanguageId { get; set; }

        
    }
}
