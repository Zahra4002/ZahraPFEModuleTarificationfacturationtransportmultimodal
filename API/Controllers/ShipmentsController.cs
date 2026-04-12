using Application.Features.ShipmentFeature.Commands;
using Application.Features.ShipmentFeature.Queries;
using Application.Features.ShipmentFeature.Validators;
using Application.Setting;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShipmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ==================== GET ====================

        [HttpGet]
        public async Task<IActionResult> GetShipments(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending = false,
            string? searchTerm = null)
        {
            var query = new GetAllShipmentsQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShipmentById(Guid id)
        {
            var query = new GetShipmentById(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}/Details")]
        public async Task<IActionResult> GetShipmentDetailsById(Guid id)
        {
            var query = new GetShipmentDetailsById(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{shipmentId}/Segments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseHttp>> GetSegmentsByShipmentId(Guid shipmentId)
        {
            var query = new GetSegmentsByShipmentIdQuery(shipmentId);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{shipmentId}/Segments/{segmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseHttp>> GetSegmentById(Guid shipmentId, Guid segmentId)
        {
            var query = new GetSegmentByIdQuery(shipmentId, segmentId);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("Status/{status}")]
        public async Task<IActionResult> GetShipmentsByStatus(ShipmentStatus status)
        {
            var query = new GetShipmentsByStatus(status);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("Client/{clientId}")]
        public async Task<IActionResult> GetShipmentsByClientId(Guid clientId)
        {
            var query = new GetShipmntById(clientId);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        // ==================== POST ====================

        [HttpPost]
        public async Task<IActionResult> AddShipment(AddShipmentCommand command)
        {
            var validator = new AddShipmentValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpPost("{shipmentId}/Segments")]
        public async Task<IActionResult> AddSegmentToShipment(Guid shipmentId, AddSegmentToShipementCommand command)
        {
            if (shipmentId != command.ShipmentId)
            {
                return BadRequest(new { Message = "ShipmentId in URL does not match command." });
            }
            var validator = new AddSegmentToShipmentValidatorCommand();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpPost("{id}/Recalculate")]
        public async Task<IActionResult> RecalculateShipment(Guid id)
        {
            var command = new RecalculateShipmentCommands(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        // ==================== PUT ====================

        [HttpPut]
        public async Task<IActionResult> UpdateShipment(UpdateShipmentCommand command)
        {
            var validator = new UpdateShipementValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{id}/Status")]
        public async Task<IActionResult> UpdateShipmentStatus(Guid id, ShipmentStatus status)
        {
            var command = new UpdateShipmentStatusCommand(id, status);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpPut("{shipmentId}/Segments/{segmentId}")]
        public async Task<IActionResult> UpdateSegmentOfShipment(Guid shipmentId, Guid segmentId, UpdateSegmentOfShipementCommand command)
        {
            if (shipmentId != command.ShipmentId || segmentId != command.SegmentId)
            {
                return BadRequest(new { Message = "Ids in URL do not match command." });
            }
            var validator = new UpdateSegmentOfShipmentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Met à jour uniquement le statut d'un segment
        /// </summary>
        [HttpPut("{shipmentId}/Segments/{segmentId}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> UpdateSegmentStatus(
            Guid shipmentId,
            Guid segmentId,
            [FromBody] UpdateSegmentStatusRequest request)
        {
            var command = new UpdateSegmentOfShipementCommand(
                ShipmentId: shipmentId,
                SegmentId: segmentId,
                Sequence: null,
                TransportMode: null,
                SupplierId: null,
                ZoneFromId: null,
                ZoneToId: null,
                DistanceKm: null,
                EstimatedTransitDays: null,
                DepartureDate: null,
                ArrivalDate: null,
                BaseCost: null,
                SurchargesTotal: null,
                TotalCost: null,
                CurrencyCode: null,
                Status: request.Status
            );

            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        // ==================== DELETE ====================

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShipment(Guid id)
        {
            var command = new DeleteShipmentCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpDelete("{shipmentId}/Segments/{segmentId}")]
        public async Task<IActionResult> DeleteSegmentOfShipment(Guid shipmentId, Guid segmentId)
        {
            var command = new DeleteSegmentOfShipementCommand(shipmentId, segmentId);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }
    }

    /// <summary>
    /// Requête pour mettre à jour le statut d'un segment
    /// </summary>
    public class UpdateSegmentStatusRequest
    {
        public SegmentStatus Status { get; set; }
    }
}