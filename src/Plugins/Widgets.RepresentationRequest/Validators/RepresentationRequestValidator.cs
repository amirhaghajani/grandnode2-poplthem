using FluentValidation;
using Grand.Business.Common.Interfaces.Localization;
using Grand.Infrastructure.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Widgets.RepresentationRequest.Models;

namespace Widgets.RepresentationRequest.Validators
{
    public class RepresentationRequestValidator : BaseGrandValidator<RepresentationRequestModel>
    {
        public RepresentationRequestValidator(IEnumerable<IValidatorConsumer<RepresentationRequestModel>> validators, 
            ITranslationService translationService) : base(validators)
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.FullName.Required"));

            RuleFor(x => x.Address).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.Address.Required"));
            RuleFor(x => x.Job).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.Job.Required"));

            RuleFor(x => x.JobExperience).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.JobExperience.Required"));
            RuleFor(x => x.WhoDidYouGetToKnowZAP).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.WhoDidYouGetToKnowZAP.Required"));
            RuleFor(x => x.StrengthAndWeakness).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.StrengthAndWeakness.Required"));

            RuleFor(x => x.EstimateOfSell).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.EstimateOfSell.Required"));
            RuleFor(x => x.SellPromotionalProgram).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.SellPromotionalProgram.Required"));
            RuleFor(x => x.WantedCities).NotEmpty().WithMessage(translationService.GetResource("Widgets.RepresentationRequest.Fields.WantedCities.Required"));
        }
    }
}
