using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // ================================
        // GET REVENUE STATS
        // ================================
        [HttpGet("revenue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRevenueStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetRevenueStatsQuery(from, to);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        // ================================
        // GET REVENUE BY CLIENT
        // ================================
        [HttpGet("revenue/by-client")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRevenueByClient([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetRevenueByClientQuery(from, to);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        [HttpGet("revenue/by-mode")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRevenueByMode([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetRevenueByModeQuery(from, to);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        // ================================
        // GET MARGIN STATS
        // ================================
        [HttpGet("margins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMarginStats([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetMarginStatsQuery(from, to);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        // ================================
        // GET INVOICE STATUS STATISTICS
        // ================================
        [HttpGet("invoices/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetInvoiceStatusStats()
        {
            var query = new GetInvoiceStatusStatsQuery();
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }

        // ================================
        // GET OVERDUE INVOICES
        // ================================
        [HttpGet("invoices/overdue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOverdueInvoices()
        {
            var query = new GetOverdueInvoicesQuery();
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : BadRequest(result);
        }


    }
}