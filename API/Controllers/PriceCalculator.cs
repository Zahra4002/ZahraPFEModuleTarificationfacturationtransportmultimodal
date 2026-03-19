using Application.Features.PriceCalculationFeature.Commands;
using Application.Features.PriceCalculationFeature.Validators;
using Application.Setting;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseHttp))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculatePrice(
            CalculatePriceCommand cmd,
            CancellationToken cancellationToken = default)
        {
            // Vérifier que la commande n'est pas nulle
            if (cmd == null)
            {
                return BadRequest(new ResponseHttp
                {
                    Status = 400,
                    Fail_Messages = "La requête ne peut pas être vide."
                });
            }

            // Valider la commande avec FluentValidation
            var validator = new CalculatePriceCommandValidator();
            var validationResult = await validator.ValidateAsync(cmd, cancellationToken);

            if (!validationResult.IsValid)
            {
                // Construire les messages d'erreur
                var errorMessages = validationResult.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();

                return BadRequest(new ResponseHttp
                {
                    Status = 400,
                    Fail_Messages = string.Join(", ", errorMessages)
                });
            }

            try
            {
                // Envoyer la commande via MediatR
                var response = await _mediator.Send(cmd, cancellationToken);

                // Retourner la réponse appropriée selon le statut
                if (response.Status == 200)
                    return Ok(response);

                if (response.Status == 404)
                    return NotFound(response);

                if (response.Status == 400)
                    return BadRequest(response);

                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseHttp
                {
                    Status = 500,
                    Fail_Messages = $"Erreur inattendue: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Calcule les prix pour plusieurs requêtes en une seule demande.
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseHttp))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CalculatePriceBulk(
            [FromBody] List<CalculatePriceCommand> commands,
            CancellationToken cancellationToken = default)
        {
            // Vérifier que la liste n'est pas vide
            if (commands == null || !commands.Any())
            {
                return BadRequest(new ResponseHttp  
                {
                    Status = 400,
                    Fail_Messages = "La liste de requêtes ne peut pas être vide."
                });
            }

            // Limiter le nombre de requêtes par bulk (par exemple 50)
            if (commands.Count > 50)
            {
                return BadRequest(new ResponseHttp
                {
                    Status = 400,
                    Fail_Messages = "Vous ne pouvez traiter que 50 calculs maximum par requête."
                });
            }

            try
            {
                var results = new List<object>();
                var validator = new CalculatePriceCommandValidator();

                // Traiter chaque commande
                foreach (var cmd in commands)
                {
                    // Valider la commande
                    var validationResult = await validator.ValidateAsync(cmd, cancellationToken);

                    if (!validationResult.IsValid)
                    {
                        results.Add(new
                        {
                            command = cmd,
                            status = 400,
                            error = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                        });
                        continue;
                    }

                    try
                    {
                        // Envoyer via MediatR
                        var response = await _mediator.Send(cmd, cancellationToken);
                        results.Add(new
                        {
                            command = cmd,
                            status = response.Status,
                            result = response.Resultat,
                            error = response.Fail_Messages
                        });
                    }
                    catch (Exception ex)
                    {
                        results.Add(new
                        {
                            command = cmd,
                            status = 500,
                            error = ex.Message
                        });
                    }
                }

                return Ok(new ResponseHttp
                {
                    Resultat = results,
                    Status = 200,
                    Fail_Messages = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseHttp
                {
                    Status = 500,
                    Fail_Messages = $"Erreur lors du traitement bulk: {ex.Message}"
                });
            }
        }
    }
}