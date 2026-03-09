using Application.Features.ShipmentFeature.Commands;
using Application.Features.ShipmentFeature.Queries;
using Application.Features.ShipmentFeature.Validators;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API pour la gestion des expéditions (shipments).
    /// </summary>
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShipmentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupère toutes les expéditions avec pagination, tri et recherche optionnels.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipments(
            int? pageNumber,
            int? pageSize,
            string? sortBy,
            bool sortDescending = false,
            string? searchTerm = null)
        {
            var query = new GetAllShipmentsQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm);
            var result = await _mediator.Send(query);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Ajoute une nouvelle expédition.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddShipment(AddShipmentCommand command)
        {
            var validator = new AddShipmentValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });

            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Récupère les expéditions par statut.
        /// </summary>
        [HttpGet("Status/{Status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentsByStatus(ShipmentStatus Status)
        {
            var query = new GetShipmentsByStatus(Status);
            var result = await _mediator.Send(query);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Récupère les expéditions pour un client donné.
        /// </summary>
        [HttpGet("Client/{CLientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentsByClientId(Guid CLientId)
        {
            var query = new GetShipmentById(CLientId);
            var result = await _mediator.Send(query);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Récupère une expédition par ID.
        /// </summary>
        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipementById(Guid Id)
        {
            var query = new GetShipmentById(Id);
            var result = await _mediator.Send(query);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Met à jour une expédition.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShipment(Guid id, UpdateShipmentCommand command)
        {
            if (id != command.ShipmentId)
                return BadRequest(new { Message = "Route id and command ShipmentId do not match." });

            var validator = new UpdateShipementValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });

            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Supprime une expédition.
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShipment(Guid id)
        {
            var command = new DeleteShipmentCommand(id);
            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Récupère les détails d'une expédition.
        /// </summary>
        [HttpGet("{id}/Details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentDetailsById(Guid id)
        {
            var query = new GetShipmentDetailsById(id);
            var result = await _mediator.Send(query);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Met à jour le statut d'une expédition.
        /// </summary>
        [HttpPut("{id}/Status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShipmentStatus(Guid id, ShipmentStatus status)
        {
            var command = new UpdateShipmentStatusCommand(id, status);
            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Recalcule une expédition.
        /// </summary>
        [HttpPost("{id}/Recalculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecalculateShipment(Guid id)
        {
            var command = new RecalculateShipmentCommands(id);
            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Ajoute un segment à une expédition.
        /// </summary>
        [HttpPost("{ShipmentId}/Segments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddSegmentToShipement(Guid ShipmentId, AddSegmentToShipementCommand command)
        {
            var validator = new AddSegmentToShipmentValidatorCommand();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });

            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Met à jour un segment d'une expédition.
        /// </summary>
        [HttpPut("{ShipmentId}/Segments/{SegmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSegmentOfShipement(Guid ShipmentId, Guid SegmentId, UpdateSegmentOfShipementCommand command)
        {
            if (ShipmentId != command.ShipmentId || SegmentId != command.SegmentId)
                return BadRequest(new { Message = "Route ids and command ids do not match." });

            var validator = new UpdateSegmentOfShipmentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });

            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }

        /// <summary>
        /// Supprime un segment d'une expédition.
        /// </summary>
        [HttpDelete("{ShipmentId}/Segments/{SegmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSegmentOfShipement(Guid ShipmentId, Guid SegmentId)
        {
            var command = new DeleteSegmentOfShipementCommand(ShipmentId, SegmentId);
            var result = await _mediator.Send(command);
            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result)
            };
        }
    }
}