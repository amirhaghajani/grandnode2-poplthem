using Grand.Domain.Localization;
using Grand.Domain.Stores;
using Grand.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grand.Infrastructure.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Widgets.RepresentationRequest.Models;

namespace Widgets.RepresentationRequest.Domain
{
    public class RepresentationRequestDomain : BaseEntity, ITranslationEntity, IStoreLinkEntity
    {
        public RepresentationRequestDomain()
        {
            Stores = new List<string>();
            Locales = new List<TranslationEntity>();
        }

        //-----------------------
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
        /// شغل
        /// </summary>
        public string Job { get; set; }


        /// <summary>
        /// سابقه فعالیت در شغل چند سال است
        /// </summary>
        public int JobExperience { get; set; }

        /// <summary>
        /// آدرس محل کار
        /// </summary>
        public string WorkAddress { get; set; }

        /// <summary>
        /// تلفن محل کار
        /// </summary>
        public string WorkTel { get; set; }


        /// <summary>
        /// وب سایت کسب و کار
        /// </summary>
        public string BusinessWebsite { get; set; }



        public string InstagramChannel { get; set; }


        /// <summary>
        /// نحوه آشنایی با زپ
        /// </summary>
        public string WhoDidYouGetToKnowZAP { get; set; }


        /// <summary>
        /// نقاط ضعف و قوت زپ چه بوده
        /// </summary>
        public string StrengthAndWeakness { get; set; }


        /// <summary>
        /// پیش بینی تعداد فروش در سال
        /// </summary>
        public int EstimateOfSell { get; set; }


        /// <summary>
        /// برنامه تبلیغاتی برای فروش
        /// </summary>
        public string SellPromotionalProgram { get; set; }


        /// <summary>
        /// شهرهایی که درخواست نمایندگی دارید
        /// </summary>
        public string WantedCities { get; set; }


        //ITranslationEntity-------------------------------------------------
        public IList<TranslationEntity> Locales { get; set; }
        //-----------------------------------------------------------------


        //IStoreLinkEntity -------------------------------------------------
        public bool LimitedToStores { get; set; }
        public IList<string> Stores { get; set; }
        //-----------------------------------------------------------------
    }
}
