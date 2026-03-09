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
    public class UpdateShipementValidator:ValidatorBase<UpdateShipmentCommand>
    {
        public UpdateShipementValidator()
        {
                RuleFor(s => s.ShipmentId)
                    .NotEmpty().WithMessage("Shipment ID is required.")
                    .Must(id => Guid.TryParse(id.ToString(), out _)).WithMessage("Invalid Shipment ID format.");
                

        }
    }
}
