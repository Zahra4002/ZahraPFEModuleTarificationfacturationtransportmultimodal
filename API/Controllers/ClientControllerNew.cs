using Application.Features.ClientFeature.Commands;
using Application.Features.ClientFeature.Queries;
using Application.Features.ClientFeature.Validators;
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
    [Route("api/client")]
    [ApiController]
    public class ClientcontrollerNew : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClientcontrollerNew(IMediator mediator)
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
        public async Task<ActionResult> Add(AddClientCommandNew cmd)
        {
            try
            {
                ResponseHttp AddCustomerResult;
                AddClientCommandNewValidator validator = new();

                AddCustomerResult = validator.Validate(new ValidationContext<AddClientCommandNew>(cmd));

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
        public async Task<ActionResult> Update([FromBody] UpdateClientCommandNew cmd)
        {
            try
            {
                ResponseHttp updateCustomerResult;
                UpdateClientCommandNewValidator validator = new();

                updateCustomerResult = validator.Validate(new ValidationContext<UpdateClientCommandNew>(cmd));

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
            var result = await _mediator.Send(new DeleteClientCommandNew(id));
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
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(Guid id)
        {
            GetClienByIdNewQuery qr = new(id);
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
            var result = await _mediator.Send(new GetAllClientNewQuery(pageNumber, pageSize));

            return Ok(result);
        }
        /// <summary>
        /// Retrieves a resource by its unique identifier.
        /// </summary>
        /// <remarks>This method sends a query to retrieve the resource associated with the specified
        /// <paramref name="code"/>. If the resource is found, it is returned with an HTTP 200 OK status. Otherwise, an
        /// appropriate HTTP error status is returned.</remarks>
        /// <param name="code">The unique identifier of the resource to retrieve.</param>
        /// <returns>An <see cref="ActionResult"/> containing the resource if found, or an appropriate HTTP response if not.</returns>
        [HttpGet("{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(string code)
        {
            GetClienByCodeNewQuery qr = new(code);
            var result = await _mediator.Send(qr);

            return Ok(result);
        }





        // API/Controllers/ClientController.cs - Ajouter cette méthode
        [HttpPost("{id}/resend-credentials")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResendCredentials(Guid id)
        {
            var result = await _mediator.Send(new ResendClientCredentialsCommand(id));
            return StatusCode(result.Status, result);
        }




        /// <summary>
        /// Récupère tous les contrats associés à un client donné
        /// </summary>
        [HttpGet("{id}/contracts")]
            [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ResponseHttp), StatusCodes.Status404NotFound)]
            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
            public async Task<ActionResult<ResponseHttp>> GetClientContracts(Guid id)
            {
                var query = new GetContractsByClientIdQuery(id);
                var result = await _mediator.Send(query);

                return StatusCode(result.Status, result);
            }
     }


 }
