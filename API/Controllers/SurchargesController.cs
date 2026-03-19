// API/Controllers/SurchargesController.cs
using Application.Features.SurchargeFeature.Commands;
using Application.Features.SurchargeFeature.Dtos;
using Application.Features.SurchargeFeature.Queries;
using Application.Features.SurchargeFeature.Validators;
using Application.Setting;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SurchargesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SurchargesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Surcharge Endpoints

        /// <summary>
        /// Retrieves a paginated list of surcharges with optional filtering
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetSurcharges(
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool? sortDescending = false,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? type = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetAllSurchargesQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm, type, isActive);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Creates a new surcharge
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateSurcharge([FromBody] CreateSurchargeDTO createSurchargeDto)
        {
            try
            {
                var command = new CreateSurchargeCommand(createSurchargeDto);
                var validator = new CreateSurchargeCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status201Created)
                    return CreatedAtAction(nameof(GetSurchargeById), new { id = ((SurchargeDTO)result.Resultat).Id }, result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Retrieves surcharges by type
        /// </summary>
        [HttpGet("type/{type}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetSurchargesByType(SurchargeType type)
        {
            var query = new GetSurchargesByTypeQuery(type.ToString());
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves a surcharge by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetSurchargeById(Guid id)
        {
            var query = new GetSurchargeByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing surcharge
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateSurcharge(Guid id, [FromBody] UpdateSurchargeDTO updateSurchargeDto)
        {
            try
            {
                var command = new UpdateSurchargeCommand(id, updateSurchargeDto);
                var validator = new UpdateSurchargeCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status200OK)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Deletes a surcharge (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeleteSurcharge(Guid id)
        {
            var command = new DeleteSurchargeCommand(id);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves detailed surcharge information including all rules
        /// </summary>
        [HttpGet("{id}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetSurchargeDetails(Guid id)
        {
            var query = new GetSurchargeDetailsQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        #endregion

        #region Surcharge Rules Endpoints

        /// <summary>
        /// Creates a new rule for a surcharge
        /// </summary>
        [HttpPost("{surchargeId}/rules")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateSurchargeRule(Guid surchargeId, [FromBody] CreateSurchargeRuleDTO createRuleDto)
        {
            try
            {
                var command = new CreateSurchargeRuleCommand(surchargeId, createRuleDto);
                var validator = new CreateSurchargeRuleCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status201Created)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Updates an existing rule
        /// </summary>
        [HttpPut("{surchargeId}/rules/{ruleId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateSurchargeRule(Guid surchargeId, Guid ruleId, [FromBody] UpdateSurchargeRuleDTO updateRuleDto)
        {
            try
            {
                var command = new UpdateSurchargeRuleCommand(surchargeId, ruleId, updateRuleDto);
                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status200OK)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Deletes a rule
        /// </summary>
        [HttpDelete("{surchargeId}/rules/{ruleId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeleteSurchargeRule(Guid surchargeId, Guid ruleId)
        {
            var command = new DeleteSurchargeRuleCommand(surchargeId, ruleId);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status204NoContent)
                return NoContent();

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        #endregion
    }
}