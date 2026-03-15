using Application.Features.SupplierFeature.Commands;
using Application.Features.SupplierFeature.Dtos;
using Application.Features.SupplierFeature.Queries;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API pour la gestion des fournisseurs.
    /// </summary>
    [Route("api/supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SupplierController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crée un nouveau fournisseur.
        /// </summary>
        /// <param name="dto">Données du fournisseur à créer.</param>
        /// <returns>Résultat de la création avec l'ID du fournisseur.</returns>
        /// <response code="201">Fournisseur créé avec succès.</response>
        /// <response code="400">Requête invalide.</response>
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
                dto.IsActive
               
            );

            var result = await _mediator.Send(command);

            if (result.Status == 201)
            {
                var id = GetIdFromResult(result);
                return CreatedAtAction(nameof(GetById), new { id }, result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Met à jour un fournisseur existant.
        /// </summary>
        /// <param name="id">ID du fournisseur à mettre à jour.</param>
        /// <param name="dto">Données mises à jour du fournisseur.</param>
        /// <returns>Résultat de la mise à jour.</returns>
        /// <response code="200">Mise à jour réussie.</response>
        /// <response code="400">Requête invalide.</response>
        /// <response code="404">Fournisseur non trouvé.</response>
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

        /// <summary>
        /// Supprime un fournisseur par ID.
        /// </summary>
        /// <param name="id">ID du fournisseur à supprimer.</param>
        /// <returns>204 si supprimé, sinon message d'erreur.</returns>
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

        /// <summary>
        /// Récupère tous les fournisseurs avec pagination optionnelle.
        /// </summary>
        /// <param name="pageNumber">Numéro de page (optionnel).</param>
        /// <param name="pageSize">Taille de page (optionnel).</param>
        /// <returns>Liste des fournisseurs.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll(int? pageNumber, int? pageSize)
        {
            var query = new GetAllSuppliersQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Récupère un fournisseur par ID.
        /// </summary>
        /// <param name="id">ID du fournisseur.</param>
        /// <returns>Détails du fournisseur.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetSupplierByIdQuery(id);
            var result = await _mediator.Send(query);

            return result.Status == 200 ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Récupère un fournisseur par son code.
        /// </summary>
        /// <param name="code">Code du fournisseur.</param>
        /// <returns>Détails du fournisseur.</returns>
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

        /// <summary>
        /// Helper pour extraire l'ID du résultat.
        /// </summary>
        private Guid GetIdFromResult(ResponseHttp result)
        {
            return result.Resultat?.GetType().GetProperty("Id")?.GetValue(result.Resultat) as Guid? ?? Guid.Empty;
        }
    }
}