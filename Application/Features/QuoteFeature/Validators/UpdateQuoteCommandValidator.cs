using Application.Common.Validator;
using Application.Features.QuoteFeature.Commands;
using FluentValidation;

namespace Application.Features.QuoteFeature.Validators
{
    public class UpdateQuoteCommandValidator : ValidatorBase<UpdateQuoteCommand>
    {
        public UpdateQuoteCommandValidator()
        {
            RuleFor(v => v.Id)
                .NotEmpty()
                .WithMessage("L'ID du devis est requis");
        }
    }
}