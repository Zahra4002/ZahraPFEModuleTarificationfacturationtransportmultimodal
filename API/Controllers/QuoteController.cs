using Application.Features.QuoteFeature.Commands;
using Application.Features.QuoteFeature.Dtos;
using Application.Features.QuoteFeature.Queries;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public QuoteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ================================
        // CREATE QUOTE
        // ================================
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateQuoteDto dto)
        {
            var command = new CreateQuoteCommand(
                dto.QuoteNumber,
                dto.ClientId,
                dto.OriginAddress,
                dto.DestinationAddress,
                dto.WeightKg,
                dto.VolumeM3,
                dto.MerchandiseTypeId,
                dto.TotalHT,
                dto.TotalTTC,
                dto.CurrencyCode,
                dto.ValidUntil,
                dto.Notes
            );

            var result = await _mediator.Send(command);

            return result.Status == 201
                ? CreatedAtAction(nameof(GetById), new { id = GetIdFromResult(result) }, result)
                : BadRequest(result);
        }

        // ================================
        // UPDATE QUOTE
        // ================================
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuoteCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest("L'ID dans l'URL ne correspond pas à l'ID dans la requête");
            }

            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // DELETE QUOTE
        // ================================
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteQuoteCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                204 => NoContent(),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // GET ALL QUOTES
        // ================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize)
        {
            var query = new GetAllQuotesQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }

        // ================================
        // GET QUOTE BY ID
        // ================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetQuoteByIdQuery(id);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }



        // ================================
        // GET QUOTES BY CLIENT ID
        // ================================
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByClientId(Guid clientId)
        {
            var query = new GetQuotesByClientIdQuery(clientId);
            var result = await _mediator.Send(query);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // ACCEPT QUOTE
        // ================================
        [HttpPost("{id}/accept")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Accept(Guid id)
        {
            var command = new AcceptQuoteCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // CONVERT QUOTE TO SHIPMENT
        // ================================
        [HttpPost("{id}/convert")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ConvertToShipment(Guid id)
        {
            var command = new ConvertQuoteToShipmentCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                201 => CreatedAtAction(nameof(GetById), new { id = GetIdFromResult(result) }, result),
                404 => NotFound(result),
                409 => Conflict(result),
                _ => BadRequest(result)
            };
        }

        // Helper pour extraire l'ID
        private Guid GetIdFromResult(ResponseHttp result)
        {
            return result.Resultat?.GetType().GetProperty("Id")?.GetValue(result.Resultat) as Guid? ?? Guid.Empty;
        }
    }
}