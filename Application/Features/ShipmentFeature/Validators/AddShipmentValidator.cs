using Application.Common.Validator;
using Application.Features.ShipmentFeature.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.ShipmentFeature.Validators
{
    
    public class AddShipmentValidator : ValidatorBase<AddShipmentCommand>
    {
        public AddShipmentValidator() {

            RuleFor(x => x.ShipmentNumber)
                .NotEmpty().WithMessage("ShipmentNumber is required.");

            RuleFor(x => x.ClientId)
                .NotNull().WithMessage("ClientId is required.");

            RuleFor(x => x.QuoteId)
                .NotNull().WithMessage("QuoteId is required.");

            RuleFor(x => x.OriginAddress)
                .NotNull().WithMessage("OriginAddress is required.");

            RuleFor(x => x.DestinationAddress)
                .NotNull().WithMessage("DestinationAddress is required.");

            RuleFor(x => x.MerchandiseTypeId)
                .NotNull().WithMessage("MerchandiseTypeId is required.");
            
            RuleFor(x => x.TotalCostHT)
                .GreaterThanOrEqualTo(0).WithMessage("TotalCostHT must be non-negative.");

            RuleFor(x => x.TotalSurcharges)
                .GreaterThanOrEqualTo(0).WithMessage("TotalSurcharges must be non-negative.");

            RuleFor(x => x.TotalTaxes)
                .GreaterThanOrEqualTo(0).WithMessage("TotalTaxes must be non-negative.");

            RuleFor(x => x.TotalCostTTC)
                .GreaterThanOrEqualTo(0).WithMessage("TotalCostTTC must be non-negative.");

            RuleFor(x => x.CurrencyCode)
                .NotEmpty().WithMessage("CurrencyCode is required.");


        }
    }
}
