using Application.Features.SupplierFeature.Commands;
using Application.Features.SupplierFeature.Dtos;
using Application.Features.SupplierFeature.Queries;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupplierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ================================
        // CREATE SUPPLIER
        // ================================
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var command = new CreateSupplierCommand(
                dto.Code,
                dto.Name,
                dto.TaxId,
                dto.Email,
                dto.Phone,
                dto.Address,
                dto.DefaultCurrencyCode,
                dto.IsActive,
                dto.Contracts,
                dto.TransportSegments
            );

            var result = await _mediator.Send(command);

            if (result.Status == 201)
            {
                var id = GetIdFromResult(result);
                return CreatedAtAction(nameof(GetById), new { id }, result);
            }

            return BadRequest(result);
        }

        // ================================
        // UPDATE SUPPLIER
        // ================================
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierDto dto)
        {
            var command = new UpdateSupplierCommand(
                id,
                dto.Code,
                dto.Name,
                dto.TaxId,
                dto.Email,
                dto.Phone,
                dto.Address,
                dto.DefaultCurrencyCode,
                dto.IsActive,
                dto.Contracts,
                dto.TransportSegments
            );

            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // DELETE SUPPLIER
        // ================================
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteSupplierCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                204 => NoContent(),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // GET ALL SUPPLIERS
        // ================================
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize)
        {
            var query = new GetAllSuppliersQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }

        // ================================
        // GET SUPPLIER BY ID
        // ================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetSupplierByIdQuery(id);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }

        // Helper pour extraire l'ID du résultat
        private Guid GetIdFromResult(ResponseHttp result)
        {
            return result.Resultat?.GetType().GetProperty("Id")?.GetValue(result.Resultat) as Guid? ?? Guid.Empty;
        }

        // ================================
        // GET SUPPLIER BY CODE
        // ================================
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByCode(string code)
        {
            var query = new GetSupplierByCodeQuery(code);
            var result = await _mediator.Send(query);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
    }
}