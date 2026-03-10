using Application.Features.ContractFeature.Commands;
using Application.Features.ContractFeature.Queries;
using Application.Features.ContractFeature.Validators;
using Application.Features.ContractFeature.Validators.Application.Features.ContractFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API de gestion des contrats.
    /// </summary>
    /// <remarks>
    /// Cette API permet de :
    /// - créer un contrat
    /// - modifier un contrat
    /// - supprimer un contrat
    /// - récupérer les contrats
    /// - gérer les prix associés aux contrats
    /// - renouveler un contrat
    /// </remarks>
    [Route("api/contract")]
    [ApiController]
    public class ContractControllerNew : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initialise une nouvelle instance du contrôleur ContractControllerNew.
        /// </summary>
        /// <param name="mediator">Service MediatR pour envoyer les commandes et requêtes.</param>
        public ContractControllerNew(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Créer un nouveau contrat.
        /// </summary>
        /// <param name="cmd">Informations du contrat à créer.</param>
        /// <returns>Résultat de la création du contrat.</returns>
        /// <response code="200">Contrat créé avec succès.</response>
        /// <response code="400">Erreur de validation.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Add(AddContractCommandNew cmd)
        {
            try
            {
                ResponseHttp result;
                AddContractCommandNewValidator validator = new();

                result = validator.Validate(new ValidationContext<AddContractCommandNew>(cmd));

                if (result.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }

                result = await _mediator.Send(cmd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mettre à jour un contrat existant.
        /// </summary>
        /// <param name="cmd">Données du contrat à mettre à jour.</param>
        /// <returns>Résultat de la mise à jour.</returns>
        /// <response code="200">Contrat mis à jour.</response>
        /// <response code="400">Erreur de validation.</response>
        [HttpPut("")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update([FromBody] UpdateContractCommandNew cmd)
        {
            try
            {
                ResponseHttp result;
                UpdateContractCommandNewValidator validator = new();

                result = validator.Validate(new ValidationContext<UpdateContractCommandNew>(cmd));

                if (result.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(result);
                }

                result = await _mediator.Send(cmd);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Supprimer un contrat.
        /// </summary>
        /// <param name="id">Identifiant du contrat.</param>
        /// <returns>Résultat de suppression.</returns>
        /// <response code="200">Contrat supprimé.</response>
        /// <response code="404">Contrat non trouvé.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteContractCommandNew(id));
            return Ok(result);
        }

        /// <summary>
        /// Récupérer un contrat par son identifiant.
        /// </summary>
        /// <param name="id">Identifiant du contrat.</param>
        /// <returns>Détails du contrat.</returns>
        /// <response code="200">Contrat trouvé.</response>
        /// <response code="404">Contrat non trouvé.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Guid id)
        {
            var result = await _mediator.Send(new GetContByIdNewQuery(id));
            return Ok(result);
        }

        /// <summary>
        /// Récupérer la liste paginée des contrats.
        /// </summary>
        /// <param name="pageNumber">Numéro de page.</param>
        /// <param name="pageSize">Nombre d’éléments par page.</param>
        /// <returns>Liste paginée des contrats.</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await _mediator.Send(new GetAllContractNewQuery(pageNumber, pageSize));
            return Ok(result);
        }

        /// <summary>
        /// Récupérer les contrats d’un client spécifique.
        /// </summary>
        /// <param name="clientId">Identifiant du client.</param>
        /// <returns>Liste des contrats du client.</returns>
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> GetByClient(Guid clientId)
        {
            var result = await _mediator.Send(new GetContractsByClientQuery(clientId));
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Récupérer les contrats qui expirent bientôt.
        /// </summary>
        /// <param name="days">Nombre de jours avant expiration.</param>
        /// <returns>Liste des contrats proches de l’expiration.</returns>
        [HttpGet("expiring")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> GetExpiring([FromQuery] int days = 30)
        {
            var result = await _mediator.Send(new GetExpiringContractsQuery(days));
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Récupérer les détails complets d’un contrat.
        /// </summary>
        /// <param name="id">Identifiant du contrat.</param>
        /// <returns>Détails complets du contrat.</returns>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> GetDetails(Guid id)
        {
            var result = await _mediator.Send(new GetContractDetailsQuery(id));
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Renouveler un contrat existant.
        /// </summary>
        /// <param name="id">Identifiant du contrat.</param>
        /// <returns>Résultat du renouvellement.</returns>
        [HttpPost("{id}/renew")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> Renew(Guid id)
        {
            var result = await _mediator.Send(new RenewContractCommand(id));
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Ajouter une tarification à un contrat.
        /// </summary>
        /// <param name="contractId">Identifiant du contrat.</param>
        /// <param name="command">Informations de la tarification.</param>
        /// <returns>Résultat de l'ajout.</returns>
        [HttpPost("{contractId}/pricings")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status201Created)]
        public async Task<ActionResult<ResponseHttp>> AddPricing(Guid contractId, [FromBody] AddContractPricingCommand command)
        {
            command = command with { ContractId = contractId };
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Modifier une tarification d’un contrat.
        /// </summary>
        /// <param name="contractId">Identifiant du contrat.</param>
        /// <param name="pricingId">Identifiant de la tarification.</param>
        /// <param name="command">Nouvelles informations de la tarification.</param>
        /// <returns>Résultat de la modification.</returns>
        [HttpPut("{contractId}/pricings/{pricingId}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> UpdatePricing(
            Guid contractId,
            Guid pricingId,
            [FromBody] UpdateContractPricingCommand command)
        {
            command = command with { ContractId = contractId, PricingId = pricingId };
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Supprimer une tarification d’un contrat.
        /// </summary>
        /// <param name="contractId">Identifiant du contrat.</param>
        /// <param name="pricingId">Identifiant de la tarification.</param>
        /// <returns>Résultat de la suppression.</returns>
        [HttpDelete("{contractId}/pricings/{pricingId}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        public async Task<ActionResult<ResponseHttp>> DeletePricing(Guid contractId, Guid pricingId)
        {
            var result = await _mediator.Send(new DeleteContractPricingCommand(contractId, pricingId));
            return StatusCode(result.Status, result);
        }
    }
}