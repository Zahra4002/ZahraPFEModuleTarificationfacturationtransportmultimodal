using Application.Features.TaxFeature.Commands;
using Application.Services;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IVatRateService _vatRateService;

        public TaxController(IMediator mediator, IVatRateService vatRateService)
        {
            _mediator = mediator;
            _vatRateService = vatRateService;
        }

        /// <summary>
        /// Importe automatiquement les taux de TVA depuis VAT Sense
        /// </summary>
        [HttpPost("fetch-from-api")]
        //[Authorize(Roles = "Administrateur")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> FetchTaxRates()
        {
            var command = new FetchTaxRatesCommand();
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Récupère le taux de TVA pour un pays
        /// </summary>
        [HttpGet("rate/{countryCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetVatRate(string countryCode)
        {
            var rate = await _vatRateService.GetVatRate(countryCode);
            return Ok(new { countryCode, vatRate = rate });
        }
    }
}