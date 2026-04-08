using Application.Common.Validator;
using Application.Features.QuoteFeature.Commands;
using FluentValidation;

namespace Application.Features.QuoteFeature.Validators
{
    public class CreateQuoteCommandValidator : ValidatorBase<CreateQuoteCommand>
    {
        public CreateQuoteCommandValidator()
        {
            RuleFor(v => v.QuoteNumber)
                .NotEmpty()
                .WithMessage("Le numéro de devis est requis");

           // RuleFor(v => v.ClientId)
              //  .NotEmpty()
              //  .WithMessage("L'ID du client est requis");

            RuleFor(v => v.ValidUntil)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La date de validité doit être dans le futur");

            RuleFor(v => v.TotalHT)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Le total HT doit être supérieur ou égal à 0");

            RuleFor(v => v.TotalTTC)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Le total TTC doit être supérieur ou égal à 0");

            RuleFor(x => x.MerchandiseTypeId)
                .NotEmpty().WithMessage("Le type de marchandise est obligatoire.");
        }
    }
}