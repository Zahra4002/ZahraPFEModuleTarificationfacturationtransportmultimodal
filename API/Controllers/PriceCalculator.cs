using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API pour le calcul des prix.
    /// </summary>
    [Route("api/price")]
    [ApiController]
    public class PriceCalculator : ControllerBase
    {
        private readonly IMediator _mediator;

        public PriceCalculator(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Calcule le prix d'un produit ou d'un service.
        /// </summary>
        /// <remarks>
        /// Exemple de requête :
        ///
        ///     POST /api/price/calculate
        ///     {
        ///        "productId": 123,
        ///        "quantity": 2
        ///     }
        ///
        /// </remarks>
        /// <returns>Le prix calculé.</returns>
        /// <response code="200">Le calcul a réussi et retourne le prix.</response>
        /// <response code="400">La requête est invalide.</response>
        /// <response code="500">Erreur serveur interne.</response>
        [HttpPost("calculate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculatePrice()
        {
            // TODO: appeler _mediator.Send avec votre commande ou query
            return Ok(new { Price = 100.0 });
        }

        /// <summary>
        /// Calcule les prix pour plusieurs produits ou services en une seule requête.
        /// </summary>
        /// <remarks>
        /// Exemple de requête :
        ///
        ///     POST /api/price/calculate/bulk
        ///     [
        ///       { "productId": 123, "quantity": 2 },
        ///       { "productId": 456, "quantity": 5 }
        ///     ]
        ///
        /// </remarks>
        /// <returns>Liste des prix calculés.</returns>
        /// <response code="200">Le calcul a réussi et retourne les prix.</response>
        /// <response code="400">La requête est invalide.</response>
        /// <response code="500">Erreur serveur interne.</response>
        [HttpPost("calculate/bulk")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculatePriceBulk()
        {
            // TODO: appeler _mediator.Send avec votre commande ou query bulk
            return Ok(new[]
            {
                new { ProductId = 123, Price = 100.0 },
                new { ProductId = 456, Price = 250.0 }
            });
        }
    }
}