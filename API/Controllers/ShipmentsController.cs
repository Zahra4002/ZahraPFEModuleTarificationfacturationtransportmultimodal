using Application.Features.ShipmentFeature.Commands;
using Application.Features.ShipmentFeature.Queries;
using Application.Features.ShipmentFeature.Validators;
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


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipments(
             int? pageNumber,
             int? pageSize,
             string? sortBy,
             bool sortDescending = false,
             string? searchTerm = null
            )
        {
            var query = new GetAllShipmentsQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm);
            var result = await _mediator.Send(query);

            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddShipment(AddShipmentCommand command)
        {

            AddShipmentValidator validator = new();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }

            var addResult = await _mediator.Send(command);

            if (addResult.Status == 200)
            {
                return Ok(addResult);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, addResult);
            }

        }

        [HttpGet("Status/{Status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentsByStatus(ShipmentStatus Status)
        {
            var query = new GetShipmentsByStatus(Status);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet("Client/{CLientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentsByClientId(Guid CLientId)
        {
            var query = new GetShipmentById(CLientId);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpGet("{Id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipementById(Guid Id)
        {
            var query = new GetShipmentById(Id);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShipment(Guid id, UpdateShipmentCommand command)
        {
            // Ensure the command uses the route id
            if (id != command.ShipmentId)
            {
                return BadRequest(new { Message = "Route id and command ShipmentId do not match." });
            }

            var validator = new UpdateShipementValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }

            var updateResult = await _mediator.Send(command);

            if (updateResult.Status == 200)
            {
                return Ok(updateResult);
            }
            else if (updateResult.Status == 404)
            {
                return NotFound(updateResult);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShipment(Guid id)
        {
            var command = new DeleteShipmentCommand(id);
            var deleteResult = await _mediator.Send(command);
            if (deleteResult.Status == 200)
            {
                return Ok(deleteResult);
            }
            else if (deleteResult.Status == 404)
            {
                return NotFound(deleteResult);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, deleteResult);
            }
        }

        [HttpGet("{id}/Details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetShipmentDetailsById(Guid id)
        {
            var query = new GetShipmentDetailsById(id);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut("{id}/Status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShipmentStatus(Guid id, ShipmentStatus status)
        {
            var command = new UpdateShipmentStatusCommand(id, status);

            var updateResult = await _mediator.Send(command);
            if (updateResult.Status == 200)
            {
                return Ok(updateResult);
            }
            else if (updateResult.Status == 404)
            {
                return NotFound(updateResult);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, updateResult);
            }
        }

        [HttpPost("{id}/Recalculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecalculateShipment(Guid id)
        {
            var command = new RecalculateShipmentCommands(id);
            var result = await _mediator.Send(command);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPost("{ShipmentId}/Segments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddSegmentToShipement(Guid ShipmentId, AddSegmentToShipementCommand command)
        {
            var validator = new AddSegmentToShipmentValidatorCommand();
            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }

            var result = await _mediator.Send(command);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }

        [HttpPut("{ShipmentId}/Segments/{SegmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSegmentOfShipement(Guid ShipmentId, Guid SegmentId, UpdateSegmentOfShipementCommand command)
        {
            if (ShipmentId != command.ShipmentId || SegmentId != command.SegmentId)
            {
                return BadRequest(new { Message = "Route ids and command ids do not match." });
            }
            var validator = new UpdateSegmentOfShipmentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(new { Message = "Validation failed.", Errors = validationResult.Errors });
            }
            var result = await _mediator.Send(command);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
        [HttpDelete("{ShipmentId}/Segments/{SegmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteSegmentOfShipement(Guid ShipmentId, Guid SegmentId)
        {
            var command = new DeleteSegmentOfShipementCommand(ShipmentId, SegmentId);
            var result = await _mediator.Send(command);
            if (result.Status == 200)
            {
                return Ok(result);
            }
            else if (result.Status == 404)
            {
                return NotFound(result);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result);
            }
        }
    }
}
