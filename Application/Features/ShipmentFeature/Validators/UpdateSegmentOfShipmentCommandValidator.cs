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
    public class UpdateSegmentOfShipmentCommandValidator : ValidatorBase<UpdateSegmentOfShipementCommand>
    {
        public UpdateSegmentOfShipmentCommandValidator() 
        {
            RuleFor(ts => ts.ShipmentId)
                   .NotEmpty().WithMessage("Shipment ID is required.")
                   .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Shipment ID format.");
            RuleFor(ts => ts.SegmentId)
                .NotEmpty().WithMessage("Segment ID is required.")
                .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Segment ID format.");
            RuleFor(Ts => Ts.TransportMode)
                .IsInEnum().WithMessage("Invalid Transport Mode.");
        }
    }
}
