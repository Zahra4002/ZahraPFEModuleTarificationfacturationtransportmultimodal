using Application.Features.ZoneFeature.Commands;
using Application.Features.ZoneFeature.Queries;
using Application.Features.ZoneFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/zone")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        public readonly IMediator _mediator;

        public ZonesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllZones(int? pageNumber, int? pageSize, string? SortedBy, bool SortDescending, string? SearchTerm)
        {
            var result = await _mediator.Send(new GetAllZoneQuery(pageNumber, pageSize, SortedBy, SortDescending, SearchTerm));

            return Ok(result);
        }

        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddZone(AddZoneCommand command)
        {
            try
            {
                ResponseHttp addZoneResult;
                AddZoneCommandValidator validator = new();

                // 1. Validate the command using the validator
                addZoneResult = validator.Validate(new ValidationContext<AddZoneCommand>(command));

                // 2. If validation fails, return BadRequest with validation result
                if (addZoneResult.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(addZoneResult);
                }

                // 3. If validation passes, send the command to the mediator (CQRS pattern)
                addZoneResult = await _mediator.Send(command);

                // 4. Return Ok with the result
                return Ok(addZoneResult);
            }
            catch (Exception ex)
            {
                // 5. If any exception occurs, return BadRequest with the exception message
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetZoneById(Guid id)
        {
            var response = await _mediator.Send(new GetZoneByIdQuery(id));
            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(new { message = response.Fail_Messages });
            }
            return Ok(response.Resultat);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteZoneCommand(id));
            return Ok(result);
        }
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateZone([FromBody] UpdateZoneCommand command)
        {
            try
            {
                var validator = new UpdateZoneValidator();
                var validationResult = validator.Validate(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var updateZoneResult = await _mediator.Send(command);

                if (updateZoneResult.Status == StatusCodes.Status404NotFound)
                {
                    return NotFound(new { message = updateZoneResult.Fail_Messages });
                }

                return Ok(updateZoneResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetZoneByCode(string code)
        {
            var response = await _mediator.Send(new GetZoneByCode(code));
            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(new { message = response.Fail_Messages });
            }
            return Ok(response.Resultat);
        }

        [HttpGet("country/{country}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetZonesByCountry(string country)
        {
            var response = await _mediator.Send(new GetZonesByCountryQuery(country));
            if (response.Status == StatusCodes.Status404NotFound)
            {
                return NotFound(new { message = response.Fail_Messages });
            }
            return Ok(response.Resultat);

        }



    }
}
