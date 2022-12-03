using Grand.Business.Cms.Interfaces;
using Grand.Business.Common.Interfaces.Directory;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Business.Common.Interfaces.Stores;
using Grand.Business.Marketing.Interfaces.Contacts;
using Grand.Business.Storage.Extensions;
using Grand.Business.Storage.Interfaces;
using Grand.Web.Common.Filters;
using Grand.Web.Common.Security.Captcha;
using Grand.Web.Common.Themes;
using Grand.Domain.Catalog;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Localization;
using Grand.Domain.Media;
using Grand.Domain.Stores;
using Grand.Domain.Tax;
using Grand.Infrastructure;
using Grand.Infrastructure.Configuration;
using Grand.Web.Commands.Models.Common;
using Grand.Web.Commands.Models.Customers;
using Grand.Web.Events;
using Grand.Web.Features.Models.Common;
using Grand.Web.Models.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Grand.Web.Controllers;

namespace Grand.Web.Themes.Popl.Controllers
{
    public class MyCommonController : BasePublicController
    {
        private readonly ITranslationService _translationService;
        private readonly ILanguageService _languageService;
        private readonly IWorkContext _workContext;
        private readonly IMediator _mediator;
        private readonly CaptchaSettings _captchaSettings;

        public MyCommonController(
            ITranslationService translationService,
            IWorkContext workContext,
            ILanguageService languageService,
            IMediator mediator,
            CaptchaSettings captchaSettings)
        {
            _translationService = translationService;
            _workContext = workContext;
            _languageService = languageService;
            _mediator = mediator;
            _captchaSettings = captchaSettings;
        }


        [Route("what-is-zap")]
        public IActionResult WhatIsZap()
        {
            return View();
        }

        [Route("why-zap")]
        public IActionResult WhyZap()
        {
            return View();
        }

        [Route("learn-zap")]
        public IActionResult LearnZap()
        {
            return View();
        }


        //این رفت توی ویدگت و اونجا براش روتینگ تعریف کردم
        //[Route("representation-request")]
        //public IActionResult RepresentationRequest()
        //{
        //    return View();
        //}


        [Route("contact-us")]
        public IActionResult ContactUs()
        {
            return View();
        }



        [HttpPost, ActionName("MyContactUsFromHome")]
        [AutoValidateAntiforgeryToken]
        [ValidateCaptcha]
        [ClosedStore(true)]
        public virtual async Task<IActionResult> ContactUsSend(
            [FromServices] StoreInformationSettings storeInformationSettings,
            [FromServices] IPageService pageService,
            ContactUsModel model, IFormCollection form, bool captchaValid)
        {
            if (storeInformationSettings.StoreClosed)
            {
                var closestorepage = await pageService.GetPageBySystemName("ContactUs");
                if (closestorepage == null || !closestorepage.AccessibleWhenStoreClosed)
                    return RedirectToRoute("StoreClosed");
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage && !captchaValid)
            {
                ModelState.AddModelError("", _captchaSettings.GetWrongCaptchaMessage(_translationService));
            }

            if (ModelState.IsValid)
            {
                var result = await _mediator.Send(new ContactUsSendCommand() {
                    CaptchaValid = captchaValid,
                    Form = form,
                    Model = model,
                    Store = _workContext.CurrentStore,
                    IpAddress = HttpContext?.Connection?.RemoteIpAddress?.ToString()
                });

                if (result.errors.Any())
                {
                    foreach (var item in result.errors)
                    {
                        ModelState.AddModelError("", item);
                    }
                }
                else
                {
                    //notification
                    await _mediator.Publish(new ContactUsEvent(_workContext.CurrentCustomer, result.model, form));

                    model = result.model;
                    return View(model);
                }
            }
            model = await _mediator.Send(new ContactUsCommand() {
                Customer = _workContext.CurrentCustomer,
                Language = _workContext.WorkingLanguage,
                Store = _workContext.CurrentStore,
                Model = model,
                Form = form
            });

            return View();
        }

    }
}
