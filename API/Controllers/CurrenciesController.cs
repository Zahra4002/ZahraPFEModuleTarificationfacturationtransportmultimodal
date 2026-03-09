using Application.Features.CurrencyFeature.Commands;
using Application.Features.CurrencyFeature.Queries;
using Application.Features.CurrencyFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {

        private readonly IMediator _mediator;

        public CurrenciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var query = new GetAllCurrenciesQuery();
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);

        }
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Add(AddCurrencyCommand command)
        {
            try
            {
                // Validate the command
                AddCurrencyCommandValidator validator = new();
                var validationResult = validator.Validate(command);

                if (!validationResult.IsValid)
                {
                    // Return validation errors
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                // Send the command to MediatR
                var addResult = await _mediator.Send(command);

                return Ok(addResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetCurrencyByIdQuery(id);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update([FromBody] UpdateCurrencyCommand command)
        {
            try
            {
                // Validate the command
                var validator = new UpdateCurrencyCommandValidator();
                var validationResult = validator.Validate(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                var updateResult = await _mediator.Send(command);

                if (updateResult.Status == 404)
                    return NotFound(updateResult);

                return Ok(updateResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCurrencyCommand(id));
            return Ok(result);
        }
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCode(string code)
        {
            var query = new GetCurrencyByCodeQuery(code);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        [HttpGet("default/{Default}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDefaultCurrencies(bool Default)
        {
            var query = new GetDefaultCurrenciesQuery(Default);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        [HttpGet("rates/{fromCode}/{ToCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRatesByFromCodeAndToCode(string fromCode, string ToCode)
        {
            var query = new GetRatesByFromCodeAndToCodeQuery(fromCode, ToCode, null, null);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);

        }
        [HttpGet("rates/{fromCode}/{ToCode}/History")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRatesHistoryByFromCodeAndToCode(string fromCode, string ToCode, DateTime? DateFrom = null, DateTime? DateTo = null)
        {
            var query = new GetRatesByFromCodeAndToCodeQuery(fromCode, ToCode, DateFrom, DateTo);
            var result = await _mediator.Send(query);
            if (result.Status == 200)
                return Ok(result);
            else if (result.Status == 404)
                return NotFound(result);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        [HttpPost("rates")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddExchangeRate(AddExchangeRateCommand command)
        {
            try
            {
                // Validate the command
                var validator = new AddExchangeRateCommandValidator();
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }
                var addResult = await _mediator.Send(command);
                if (addResult.Status == 404)
                    return NotFound(addResult);
                return Ok(addResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }


        }
        [HttpPost("rates/bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddManyExchangeRates(AddManyExchagesCommand command)
        {
            try
            {
                var addResult = await _mediator.Send(command);
                if (addResult.Status != 200)
                    return BadRequest(addResult);
                return Ok(addResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }
        }

        [HttpPost("convert")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConvertAmount(ConverMoutCommand command)
        {
            try
            {
                // Validate the command
                var validator = new ConverMoutCommandValidator();
                var validationResult = validator.Validate(command);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }
                var convertResult = await _mediator.Send(command);
                if (convertResult.Status == 404)
                    return NotFound(convertResult);
                return Ok(convertResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
            }

        }

    }
    
}
