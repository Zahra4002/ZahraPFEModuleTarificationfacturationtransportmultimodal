using Application.Features.MerchandiseTypeFeature.Commands;
using Application.Features.MerchandiseTypeFeature.Queries;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchandiseTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MerchandiseTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupère tous les types de marchandises (paginated)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> GetAll(
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDescending = false,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool? isActive = null)
        {
            var query = new GetAllMerchandiseTypesQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm, isActive);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Récupère un type de marchandise par son ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> GetById(Guid id)
        {
            var query = new GetMerchandiseTypeByIdQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Récupère un type de marchandise par son code
        /// </summary>
        [HttpGet("code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> GetByCode(string code)
        {
            var query = new GetMerchandiseTypeByCodeQuery(code);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Crée un nouveau type de marchandise
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> Create([FromBody] CreateMerchandiseTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Met à jour un type de marchandise existant (PUT avec ID dans l'URL)
        /// </summary>
        /// <param name="id">ID du type de marchandise</param>
        /// <param name="command">Données du type de marchandise</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ResponseHttp>> Update(Guid id, [FromBody] UpdateMerchandiseTypeCommand command)
        {
            // Vérifier que l'ID dans l'URL correspond à l'ID dans le body
            if (id != command.Id)
            {
                return BadRequest(new ResponseHttp
                {
                    Status = StatusCodes.Status400BadRequest,
                    Fail_Messages = "L'ID dans l'URL ne correspond pas à l'ID dans le corps de la requête."
                });
            }

            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        /// <summary>
        /// Supprime un type de marchandise (soft delete)
        /// </summary>
        /// <param name="id">ID du type de marchandise</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseHttp>> Delete(Guid id)
        {
            var command = new DeleteMerchandiseTypeCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }
    }
}