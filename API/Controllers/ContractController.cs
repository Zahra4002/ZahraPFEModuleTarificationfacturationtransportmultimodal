using Application.Features.ContractFeature.Commands;
using Application.Features.ContractFeature.Queries;
using Application.Features.ContractFeature.Validators;
using Application.Features.ContractFeature.Validators.Application.Features.ContractFeature.Validators;
using Application.Features.CustomerFeatures.Validators;
using Application.Features.TestFeature.Commands;
using Application.Features.TestFeature.Queries;
using Application.Features.TestFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/contract")]
    [ApiController]
    public class ContractControllerNew : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContractControllerNew(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Handles the addition of a new test entity based on the provided command.
        /// </summary>
        /// <remarks>This method validates the input command before processing it. If the validation
        /// fails,  a 400 Bad Request response is returned. Otherwise, the command is sent to the mediator  for further
        /// processing.</remarks>
        /// <param name="cmd">The command containing the details of the test entity to be added.</param>
        /// <returns>An <see cref="ActionResult"/> containing the result of the operation.  Returns <see
        /// cref="BadRequestObjectResult"/> if the validation fails or an exception occurs.  Returns <see
        /// cref="OkObjectResult"/> with the operation result if the addition is successful.</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Add(AddContractCommandNew cmd)
        {
            try
            {
                ResponseHttp AddCustomerResult;
                AddContractCommandNewValidator validator = new();

                AddCustomerResult = validator.Validate(new ValidationContext<AddContractCommandNew>(cmd));

                if (AddCustomerResult.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(AddCustomerResult);
                }

                AddCustomerResult = await _mediator.Send(cmd);

                return Ok(AddCustomerResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Updates an existing resource based on the provided command.
        /// </summary>
        /// <remarks>This method validates the input command before processing the update. If validation
        /// fails, a  <see cref="BadRequestObjectResult"/> is returned with the validation details. If the update is
        /// successful,  the result of the operation is returned in an <see cref="OkObjectResult"/>.</remarks>
        /// <param name="cmd">The command containing the data required to update the resource. This must include all necessary fields for
        /// validation and processing.</param>
        /// <returns>An <see cref="ActionResult"/> representing the result of the operation. Returns: <list type="bullet"> <item>
        /// <description><see cref="BadRequestObjectResult"/> if the validation fails or an exception
        /// occurs.</description> </item> <item> <description><see cref="OkObjectResult"/> if the update operation
        /// completes successfully.</description> </item> </list></returns>
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update([FromBody] UpdateContractCommandNew cmd)
        {
            try
            {
                ResponseHttp updateCustomerResult;
                UpdateContractCommandNewValidator validator = new();

                updateCustomerResult = validator.Validate(new ValidationContext<UpdateContractCommandNew>(cmd));

                if (updateCustomerResult.Status == StatusCodes.Status400BadRequest)
                {
                    return BadRequest(updateCustomerResult);
                }

                updateCustomerResult = await _mediator.Send(cmd);

                return Ok(updateCustomerResult);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// Deletes the resource identified by the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier of the resource to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains  <see langword="true"/> if the
        /// resource was successfully deleted; otherwise,  <see langword="false"/>.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<bool>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteContractCommandNew(id));
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a resource by its unique identifier.
        /// </summary>
        /// <remarks>This method sends a query to retrieve the resource associated with the specified
        /// <paramref name="id"/>. If the resource is found, it is returned with an HTTP 200 OK status. Otherwise, an
        /// appropriate HTTP error status is returned.</remarks>
        /// <param name="id">The unique identifier of the resource to retrieve.</param>
        /// <returns>An <see cref="ActionResult"/> containing the resource if found, or an appropriate HTTP response if not.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(Guid id)
        {
            GetContByIdNewQuery qr = new(id);
            var result = await _mediator.Send(qr);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves a paginated list of test items.
        /// </summary>
        /// <remarks>The pagination parameters allow the caller to specify which subset of the data to
        /// retrieve. If both <paramref name="pageNumber"/> and <paramref name="pageSize"/> are null, default values are
        /// applied.</remarks>
        /// <param name="pageNumber">The page number to retrieve. If null, the default page is returned.</param>
        /// <param name="pageSize">The number of items per page. If null, the default page size is used.</param>
        /// <returns>An <see cref="ActionResult"/> containing the paginated list of test items.</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(int? pageNumber, int? pageSize)
        {
            var result = await _mediator.Send(new GetAllContractNewQuery(pageNumber, pageSize));

            return Ok(result);
        }
        // GET /api/Contracts/client/{clientId}
        [HttpGet("client/{clientId}")]
        public async Task<ActionResult<ResponseHttp>> GetByClient(Guid clientId)
        {
            var query = new GetContractsByClientQuery(clientId);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        // GET /api/Contracts/expiring
        [HttpGet("expiring")]
        public async Task<ActionResult<ResponseHttp>> GetExpiring([FromQuery] int days = 30)
        {
            var query = new GetExpiringContractsQuery(days);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        // GET /api/Contracts/{id}/details (avec pricings + client/supplier)
        [HttpGet("{id}/details")]
        public async Task<ActionResult<ResponseHttp>> GetDetails(Guid id)
        {
            var query = new GetContractDetailsQuery(id);
            var result = await _mediator.Send(query);
            return StatusCode(result.Status, result);
        }

        // POST /api/Contracts/{id}/renew
        [HttpPost("{id}/renew")]
        public async Task<ActionResult<ResponseHttp>> Renew(Guid id)
        {
            var command = new RenewContractCommand(id);
            var result = await _mediator.Send(command);
            return StatusCode(result.Status, result);
        }

        [HttpPost("{contractId}/pricings")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> AddPricing(
            Guid contractId,
            [FromBody] AddContractPricingCommand command)
        {
            try
            {
                // Validation manuelle (comme dans tes autres controllers)
                var validator = new AddContractPricingCommandValidator(); // ← crée ce validator si besoin
                var validationResult = validator.Validate(new ValidationContext<AddContractPricingCommand>(command));

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" ; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Erreurs de validation : {errors}"
                    });
                }

                // Assigner l'ID du contrat depuis l'URL
                command = command with { ContractId = contractId };

                var result = await _mediator.Send(command);
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = ex.Message
                });
            }
        }

        [HttpPut("{contractId}/pricings/{pricingId}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdatePricing(
             Guid contractId,
             Guid pricingId,
             [FromBody] UpdateContractPricingCommand command)
        {
            try
            {
                // Validation manuelle
                var validator = new UpdateContractPricingCommandValidator(); // ← crée ce validator si besoin
                var validationResult = validator.Validate(new ValidationContext<UpdateContractPricingCommand>(command));

                if (!validationResult.IsValid)
                {
                    var errors = string.Join(" ; ", validationResult.Errors.Select(e => e.ErrorMessage));
                    return BadRequest(new ResponseHttp
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Fail_Messages = $"Erreurs de validation : {errors}"
                    });
                }

                // Assigner les IDs depuis l'URL
                command = command with { ContractId = contractId, PricingId = pricingId };

                var result = await _mediator.Send(command);
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = ex.Message
                });
            }
        }

        [HttpDelete("{contractId}/pricings/{pricingId}")]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeletePricing(Guid contractId, Guid pricingId)
        {
            try
            {
                var command = new DeleteContractPricingCommand(contractId, pricingId);
                var result = await _mediator.Send(command);
                return StatusCode(result.Status, result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHttp
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Fail_Messages = ex.Message
                });
            }
        }

    }
}
