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
    public class AddSegmentToShipmentValidatorCommand : ValidatorBase<AddSegmentToShipementCommand>
    {
        public AddSegmentToShipmentValidatorCommand() 
        { 
                RuleFor(x => x.ShipmentId).NotEmpty().WithMessage("ShipmentId is required.");
                RuleFor(x => x.SupplierId).NotEmpty().WithMessage("SupplierId is required.");
                RuleFor(x => x.ZoneFromId).NotEmpty().WithMessage("ZoneFromId is required.");
                RuleFor(x => x.ZoneToId).NotEmpty().WithMessage("ZoneToId is required.");
                RuleFor(x => x.DistanceKm).GreaterThan(0).WithMessage("DistanceKm must be greater than 0.");
                RuleFor(x => x.EstimatedTransitDays).GreaterThan(0).WithMessage("EstimatedTransitDays must be greater than 0.");
                RuleFor(x => x.BaseCost).GreaterThanOrEqualTo(0).WithMessage("BaseCost must be greater than or equal to 0.");
                RuleFor(x => x.SurchargesTotal).GreaterThanOrEqualTo(0).WithMessage("SurchargesTotal must be greater than or equal to 0.");
                RuleFor(x => x.TotalCost).GreaterThanOrEqualTo(0).WithMessage("TotalCost must be greater than or equal to 0.");
                RuleFor(x => x.CurrencyCode).NotEmpty().WithMessage("CurrencyCode is required.");
        }
    }
}
