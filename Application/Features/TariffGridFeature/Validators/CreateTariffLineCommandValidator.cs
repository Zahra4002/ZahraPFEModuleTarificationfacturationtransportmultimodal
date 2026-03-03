// Application/Features/TariffGridFeature/Validators/CreateTariffLineCommandValidator.cs
using Application.Common.Validator;
using Application.Features.TariffGridFeature.Commands;
using FluentValidation;

namespace Application.Features.TariffGridFeature.Validators
{
    public class CreateTariffLineCommandValidator : ValidatorBase<CreateTariffLineCommand>
    {
        public CreateTariffLineCommandValidator()
        {
            RuleFor(v => v.GridId)
                .NotEmpty().WithMessage("Grid ID is required");

            RuleFor(v => v.LineDto.ZoneFromId)
                .NotEmpty().WithMessage("Zone from ID is required");

            RuleFor(v => v.LineDto.ZoneToId)
                .NotEmpty().WithMessage("Zone to ID is required")
                .NotEqual(v => v.LineDto.ZoneFromId).WithMessage("Zone from and zone to must be different");

            // At least one pricing method should be provided
            RuleFor(v => v.LineDto)
                .Must(dto => dto.PricePerKg.HasValue ||
                             dto.PricePerM3.HasValue ||
                             dto.PricePerContainer20ft.HasValue ||
                             dto.PricePerContainer40ft.HasValue ||
                             dto.PricePerLinearMeter.HasValue ||
                             dto.BasePrice.HasValue)
                .WithMessage("At least one pricing method must be provided");

            // Validate ranges
            When(v => v.LineDto.MinWeight.HasValue && v.LineDto.MaxWeight.HasValue, () =>
            {
                RuleFor(v => v.LineDto.MinWeight)
                    .LessThanOrEqualTo(v => v.LineDto.MaxWeight)
                    .WithMessage("Min weight must be less than or equal to max weight");
            });

            When(v => v.LineDto.MinVolume.HasValue && v.LineDto.MaxVolume.HasValue, () =>
            {
                RuleFor(v => v.LineDto.MinVolume)
                    .LessThanOrEqualTo(v => v.LineDto.MaxVolume)
                    .WithMessage("Min volume must be less than or equal to max volume");
            });

            When(v => v.LineDto.MinHeight.HasValue && v.LineDto.MaxHeight.HasValue, () =>
            {
                RuleFor(v => v.LineDto.MinHeight)
                    .LessThanOrEqualTo(v => v.LineDto.MaxHeight)
                    .WithMessage("Min height must be less than or equal to max height");
            });

            // Transit days must be positive
            When(v => v.LineDto.TransitDays.HasValue, () =>
            {
                RuleFor(v => v.LineDto.TransitDays)
                    .GreaterThan(0).WithMessage("Transit days must be greater than 0");
            });
        }
    }
}