// API/Controllers/TariffGridsController.cs
using Application.Features.TariffGridFeature.Commands;
using Application.Features.TariffGridFeature.Dtos;
using Application.Features.TariffGridFeature.Queries;
using Application.Features.TariffGridFeature.Validators;
using Application.Setting;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Uncomment when authentication is implemented
    public class TariffGridsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TariffGridsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Tariff Grid Endpoints

        /// <summary>
        /// Retrieves a paginated list of tariff grids with optional filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="sortBy">Sort field (code, name, transportMode, validFrom, validTo, createdAt)</param>
        /// <param name="sortDescending">Sort in descending order</param>
        /// <param name="searchTerm">Search term for code, name, or description</param>
        /// <returns>Paginated list of tariff grids</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetTariffGrids(
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool? sortDescending = false,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllTariffGridsQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Creates a new tariff grid
        /// </summary>
        /// <param name="createTariffGridDto">Tariff grid creation data</param>
        /// <returns>Created tariff grid</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateTariffGrid([FromBody] CreateTariffGridDTO createTariffGridDto)
        {
            try
            {
                var command = new CreateTariffGridCommand(createTariffGridDto);
                var validator = new CreateTariffGridCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status201Created)
                    return CreatedAtAction(nameof(GetTariffGridById), new { id = ((TariffGridDTO)result.Resultat).Id }, result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Retrieves a tariff grid by ID
        /// </summary>
        /// <param name="id">Tariff grid ID</param>
        /// <returns>Tariff grid details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetTariffGridById(Guid id)
        {
            var query = new GetTariffGridByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing tariff grid
        /// </summary>
        /// <param name="id">Tariff grid ID</param>
        /// <param name="updateTariffGridDto">Tariff grid update data</param>
        /// <returns>Updated tariff grid</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateTariffGrid(Guid id, [FromBody] UpdateTariffGridDTO updateTariffGridDto)
        {
            try
            {
                var command = new UpdateTariffGridCommand(id, updateTariffGridDto);
                var validator = new UpdateTariffGridCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status200OK)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Deletes a tariff grid (soft delete)
        /// </summary>
        /// <param name="id">Tariff grid ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeleteTariffGrid(Guid id)
        {
            var command = new DeleteTariffGridCommand(id);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves detailed tariff grid information including all lines
        /// </summary>
        /// <param name="id">Tariff grid ID</param>
        /// <returns>Tariff grid details with lines</returns>
        [HttpGet("{id}/details")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetTariffGridDetails(Guid id)
        {
            var query = new GetTariffGridDetailsQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves tariff grids by transport mode
        /// </summary>
        /// <param name="mode">Transport mode (Terrestre, Maritime, RoRo, Conteneurise)</param>
        /// <returns>List of tariff grids for the specified mode</returns>
        [HttpGet("mode/{mode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetTariffGridsByMode(TransportMode mode)
        {
            var query = new GetTariffGridsByModeQuery(mode.ToString());
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Clones an existing tariff grid
        /// </summary>
        /// <param name="id">Tariff grid ID to clone</param>
        /// <returns>Cloned tariff grid</returns>
        [HttpPost("{id}/clone")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CloneTariffGrid(Guid id, [FromQuery] string? newCode = null, [FromQuery] string? newName = null)
        {
            var command = new CloneTariffGridCommand(id, newCode, newName);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status201Created)
                return CreatedAtAction(nameof(GetTariffGridById), new { id = ((TariffGridDTO)result.Resultat).Id }, result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Retrieves version history for a tariff grid by code
        /// </summary>
        /// <param name="code">Tariff grid code</param>
        /// <returns>List of all versions of the tariff grid</returns>
        [HttpGet("history/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetTariffGridHistory(string code)
        {
            var query = new GetTariffGridHistoryQuery(code);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        #endregion

        #region Tariff Lines Endpoints

        /// <summary>
        /// Creates a new tariff line in a grid
        /// </summary>
        /// <param name="gridId">Tariff grid ID</param>
        /// <param name="createTariffLineDto">Tariff line creation data</param>
        /// <returns>Created tariff line</returns>
        [HttpPost("{gridId}/lines")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateTariffLine(Guid gridId, [FromBody] CreateTariffLineDTO createTariffLineDto)
        {
            try
            {
                var command = new CreateTariffLineCommand(gridId, createTariffLineDto);
                var validator = new CreateTariffLineCommandValidator();
                var validationResult = await validator.ValidateAsync(command);

                if (!validationResult.IsValid)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)),
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status201Created)
                    return Created($"api/TariffGrids/{gridId}/lines/{((TariffLineDTO)result.Resultat).Id}", result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Creates multiple tariff lines in a grid (bulk operation)
        /// </summary>
        /// <param name="gridId">Tariff grid ID</param>
        /// <param name="bulkDto">Bulk tariff line creation data</param>
        /// <returns>Created tariff lines</returns>
        [HttpPost("{gridId}/lines/bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateTariffLinesBulk(Guid gridId, [FromBody] CreateTariffLineBulkDTO bulkDto)
        {
            try
            {
                if (bulkDto.Lines == null || !bulkDto.Lines.Any())
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "No lines provided",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var command = new CreateTariffLinesBulkCommand(gridId, bulkDto);
                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status201Created)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Updates an existing tariff line
        /// </summary>
        /// <param name="gridId">Tariff grid ID</param>
        /// <param name="lineId">Tariff line ID</param>
        /// <param name="updateTariffLineDto">Tariff line update data</param>
        /// <returns>Updated tariff line</returns>
        [HttpPut("{gridId}/lines/{lineId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateTariffLine(Guid gridId, Guid lineId, [FromBody] UpdateTariffLineDTO updateTariffLineDto)
        {
            try
            {
                var command = new UpdateTariffLineCommand(gridId, lineId, updateTariffLineDto);
                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status200OK)
                    return Ok(result);

                if (result.Status == StatusCodes.Status404NotFound)
                    return NotFound(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Deletes a tariff line
        /// </summary>
        /// <param name="gridId">Tariff grid ID</param>
        /// <param name="lineId">Tariff line ID</param>
        /// <returns>No content</returns>
        [HttpDelete("{gridId}/lines/{lineId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeleteTariffLine(Guid gridId, Guid lineId)
        {
            var command = new DeleteTariffLineCommand(gridId, lineId);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status204NoContent)
                return NoContent();

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        #endregion
    }
}