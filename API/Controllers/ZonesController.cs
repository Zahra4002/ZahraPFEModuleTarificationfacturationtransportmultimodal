using Application.Features.ZoneFeature.Commands;
using Application.Features.ZoneFeature.Queries;
using Application.Features.ZoneFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API pour la gestion des zones.
    /// </summary>
    [Route("api/zone")]
    [ApiController]
    public class ZonesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ZonesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupère toutes les zones avec pagination et filtrage optionnel.
        /// </summary>
        /// <param name="pageNumber">Numéro de la page (optionnel).</param>
        /// <param name="pageSize">Taille de la page (optionnel).</param>
        /// <param name="SortedBy">Nom du champ pour trier (optionnel).</param>
        /// <param name="SortDescending">Tri décroissant si vrai.</param>
        /// <param name="SearchTerm">Mot-clé de recherche (optionnel).</param>
        /// <returns>Liste des zones.</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllZones(int? pageNumber, int? pageSize, string? SortedBy, bool SortDescending, string? SearchTerm)
        {
            var result = await _mediator.Send(new GetAllZoneQuery(pageNumber, pageSize, SortedBy, SortDescending, SearchTerm));
            return Ok(result);
        }

        /// <summary>
        /// Récupère tous les pays pour la liste déroulante
        /// </summary>
        [HttpGet("countries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetCountries()
        {
            var query = new GetCountriesQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Ajoute une nouvelle zone.
        /// </summary>
        /// <param name="command">Détails de la zone à ajouter.</param>
        /// <returns>Résultat de l'ajout.</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddZone(AddZoneCommand command)
        {
            try
            {
                var validator = new AddZoneCommandValidator();
                var validationResult = validator.Validate(new ValidationContext<AddZoneCommand>(command));

                if (validationResult.Status == StatusCodes.Status400BadRequest)
                    return BadRequest(validationResult);

                var addZoneResult = await _mediator.Send(command);
                return Ok(addZoneResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Récupère une zone par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant de la zone.</param>
        /// <returns>Détails de la zone.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetZoneById(Guid id)
        {
            var response = await _mediator.Send(new GetZoneByIdQuery(id));
            if (response.Status == StatusCodes.Status404NotFound)
                return NotFound(new { message = response.Fail_Messages });

            return Ok(response.Resultat);
        }

        /// <summary>
        /// Supprime une zone.
        /// </summary>
        /// <param name="id">Identifiant de la zone à supprimer.</param>
        /// <returns>Succès ou échec.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteZoneCommand(id));
            return Ok(result);
        }

        /// <summary>
        /// Met à jour une zone existante.
        /// </summary>
        /// <param name="command">Détails de la zone à mettre à jour.</param>
        /// <returns>Résultat de la mise à jour.</returns>
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
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });

                var updateZoneResult = await _mediator.Send(command);

                if (updateZoneResult.Status == StatusCodes.Status404NotFound)
                    return NotFound(new { message = updateZoneResult.Fail_Messages });

                return Ok(updateZoneResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Récupère une zone par son code.
        /// </summary>
        /// <param name="code">Code de la zone.</param>
        /// <returns>Détails de la zone.</returns>
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetZoneByCode(string code)
        {
            var response = await _mediator.Send(new GetZoneByCode(code));
            if (response.Status == StatusCodes.Status404NotFound)
                return NotFound(new { message = response.Fail_Messages });

            return Ok(response.Resultat);
        }

        /// <summary>
        /// Récupère toutes les zones d’un pays donné.
        /// </summary>
        /// <param name="country">Nom du pays.</param>
        /// <returns>Liste des zones pour le pays.</returns>
        [HttpGet("country/{country}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetZonesByCountry(string country)
        {
            var response = await _mediator.Send(new GetZonesByCountryQuery(country));
            if (response.Status == StatusCodes.Status404NotFound)
                return NotFound(new { message = response.Fail_Messages });

            return Ok(response.Resultat);
        }
    }
}