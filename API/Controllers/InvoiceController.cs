using Application.Features.InvoiceFeature.Commands;
using Application.Features.InvoiceFeature.Dtos;
using Application.Features.InvoiceFeature.Queries;
using Application.Features.InvoiceFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly object GetById;

        public InvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ================================
        // ADD INVOICE
        // ================================
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Add(AddInvoiceCommand cmd)
        {
            try
            {
                ResponseHttp result;
                AddInvoiceCommandValidator validator = new();

                result = validator.Validate(new ValidationContext<AddInvoiceCommand>(cmd));

                if (result.Status == StatusCodes.Status400BadRequest)
                    return BadRequest(result);

                result = await _mediator.Send(cmd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================
        // UPDATE INVOICE
        // ================================
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update([FromBody] UpdateInvoiceCommand cmd)
        {
            try
            {
                ResponseHttp result;
                UpdateInvoiceCommandValidator validator = new();

                result = validator.Validate(new ValidationContext<UpdateInvoiceCommand>(cmd));

                if (result.Status == StatusCodes.Status400BadRequest)
                    return BadRequest(result);

                result = await _mediator.Send(cmd);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ================================
        // DELETE INVOICE
        // ================================
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteInvoiceCommand(id));
            return Ok(result);
        }

        // ================================
        // GET INVOICE BY ID
        // ================================
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetInvoiceByIdNewQuery(id));
            return Ok(result);
        }

        // ================================
        // GET ALL INVOICES (Pagination)
        // ================================
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await _mediator.Send(new GetAllInvoiceQuery(pageNumber, pageSize));
            return Ok(result);
        }

        // ================================
        // GET OVERDUE INVOICES
        // ================================
        [HttpGet("overdue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOverdue()
        {
            var query = new GetOverdueInvoicesQuery();
            var result = await _mediator.Send(query);

            return result.Status == 200
                ? Ok(result)
                : BadRequest(result);
        }

        // ================================
        // CREATE INVOICE FROM SHIPMENT
        // ================================
        [HttpPost("from-shipment")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateFromShipment([FromBody] CreateInvoiceFromShipmentDto dto)
        {
            var command = new CreateInvoiceFromShipmentCommand(dto.ShipmentId, dto.CurrencyId);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                201 => CreatedAtAction(nameof(Get), new { id = result.Resultat?.GetType().GetProperty("Id")?.GetValue(result.Resultat) }, result),
                404 => NotFound(result),
                409 => Conflict(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // ✅ EMIT INVOICE (NOUVEAU)
        // ================================
        [HttpPost("{id}/emit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Emit(Guid id)
        {
            var command = new EmitInvoiceCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusDto dto)
        {
            var command = new UpdateInvoiceStatusCommand(id, dto.Status);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
        [HttpPost("{id}/credit-note")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCreditNote(Guid id)
        {
            var command = new CreateCreditNoteCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                201 => CreatedAtAction(nameof(Get), new { id = GetIdFromResult(result) }, result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // Helper pour extraire l'ID
        private Guid? GetIdFromResult(ResponseHttp result)
        {
            return result.Resultat?.GetType().GetProperty("Id")?.GetValue(result.Resultat) as Guid?;
        }
        // ================================
        // RECALCULATE INVOICE TOTALS
        // ================================
        [HttpPost("{id}/recalculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Recalculate(Guid id)
        {
            var command = new RecalculateInvoiceCommand(id);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                200 => Ok(result),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

        // ================================
        // GET INVOICE PDF
        // ================================
        [HttpGet("{id}/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPdf(Guid id)
        {
            var query = new GetInvoicePdfQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == 200 && result.Resultat != null)
            {
                // ✅ Cast explicite au lieu de dynamic
                var pdfData = result.Resultat as PdfResponseDto;
                if (pdfData != null)
                {
                    byte[] fileBytes = Convert.FromBase64String(pdfData.FileContent);
                    return File(fileBytes, pdfData.ContentType, pdfData.FileName);
                }
            }

            return result.Status switch
            {
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }
        // ================================
        // ADD INVOICE LINE
        // ================================
        [HttpPost("{invoiceId}/lines")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddLine(Guid invoiceId, [FromBody] AddInvoiceLineDto dto)
        {
            var command = new AddInvoiceLineCommand(
                invoiceId,
                dto.Description,
                dto.Quantity,
                dto.Unit,
                dto.UnitPriceHT,
                dto.DiscountPercent,
                dto.VatRate,
                dto.TransportSegmentId,
                dto.SurchargeId
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
        // UPDATE INVOICE LINE
        // ================================
        [HttpPut("{invoiceId}/lines/{lineId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLine(Guid invoiceId, Guid lineId, [FromBody] UpdateInvoiceLineDto dto)
        {
            var command = new UpdateInvoiceLineCommand(
                invoiceId,
                lineId,
                dto.Description,
                dto.Quantity,
                dto.Unit,
                dto.UnitPriceHT,
                dto.DiscountPercent,
                dto.VatRate
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
        // DELETE INVOICE LINE
        // ================================
        [HttpDelete("{invoiceId}/lines/{lineId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteLine(Guid invoiceId, Guid lineId)
        {
            var command = new DeleteInvoiceLineCommand(invoiceId, lineId);
            var result = await _mediator.Send(command);

            return result.Status switch
            {
                204 => NoContent(),
                404 => NotFound(result),
                _ => BadRequest(result)
            };
        }

   


    }

}
