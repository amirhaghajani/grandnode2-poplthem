using Grand.Business.Common.Extensions;
using Grand.Infrastructure;
using Grand.Web.Common.Components;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.FAQs.Domain;
using Widgets.FAQs.Models;
using Widgets.FAQs.Services;

namespace Widgets.FAQs.Components
{
    [ViewComponent(Name = "WidgetFAQs")]
    public class WidgetFAQsComponent : ViewComponent
    {
        private readonly IFAQsService _faqService;
        private readonly IWorkContext _workContext;

        public WidgetFAQsComponent(IWorkContext wc, IFAQsService service )
        {
            this._faqService = service;
            this._workContext = wc;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetZone, object additionalData = null)
        {
            IList<FAQ> answers;

            var inHomePage = widgetZone == "home_page_faqs";

            if (inHomePage)
            {
                answers = await this._faqService.GetFaqs(true);
            }
            else
            {
                answers = await this._faqService.GetFaqs();
            }


            var model = new PublicInfoModel();

            model.ShowLinkToAllFAQs = inHomePage;

            foreach (var item in answers)
            {
                model.FAQsList.Add(new PublicInfoModel.PublicFAQ {
                    Question = item.GetTranslation(x=>x.Question, _workContext.WorkingLanguage.Id),
                    Answer = item.GetTranslation(x => x.Answer, _workContext.WorkingLanguage.Id),
                });
            }
            

            return View(this.GetViewPath(), model);
        }
    }
}
