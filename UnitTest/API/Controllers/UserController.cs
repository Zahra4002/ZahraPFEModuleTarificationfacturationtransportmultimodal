//using Application.Setting;
//using FluentValidation;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//    [Route("api/User")]
//    [ApiController]
//    public class UserControllerNew : ControllerBase
//    {
//        private readonly IMediator _mediator;

//        public UserControllerNew(IMediator mediator)
//        {
//            _mediator = mediator;
//        }
//        /// <summary>
//        /// Handles the addition of a new User entity based on the provided command.
//        /// </summary>
//        /// <remarks>This method validates the input command before processing it. If the validation
//        /// fails,  a 400 Bad Request response is returned. Otherwise, the command is sent to the mediator  for further
//        /// processing.</remarks>
//        /// <param name="cmd">The command containing the details of the User entity to be added.</param>
//        /// <returns>An <see cref="ActionResult"/> containing the result of the operation.  Returns <see
//        /// cref="BadRequestObjectResult"/> if the validation fails or an exception occurs.  Returns <see
//        /// cref="OkObjectResult"/> with the operation result if the addition is successful.</returns>
//        [HttpPost("")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<ActionResult> Add(AddUserCommandNew cmd)
//        {
//            try
//            {
//                ResponseHttp AddCustomerResult;
//                AddUserCommandNewValidator validator = new();

//                AddCustomerResult = validator.Validate(new ValidationContext<AddUserCommandNew>(cmd));

//                if (AddCustomerResult.Status == StatusCodes.Status400BadRequest)
//                {
//                    return BadRequest(AddCustomerResult);
//                }

//                AddCustomerResult = await _mediator.Send(cmd);

//                return Ok(AddCustomerResult);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }
//        }
//        /// <summary>
//        /// Updates an existing resource based on the provided command.
//        /// </summary>
//        /// <remarks>This method validates the input command before processing the update. If validation
//        /// fails, a  <see cref="BadRequestObjectResult"/> is returned with the validation details. If the update is
//        /// successful,  the result of the operation is returned in an <see cref="OkObjectResult"/>.</remarks>
//        /// <param name="cmd">The command containing the data required to update the resource. This must include all necessary fields for
//        /// validation and processing.</param>
//        /// <returns>An <see cref="ActionResult"/> representing the result of the operation. Returns: <list type="bullet"> <item>
//        /// <description><see cref="BadRequestObjectResult"/> if the validation fails or an exception
//        /// occurs.</description> </item> <item> <description><see cref="OkObjectResult"/> if the update operation
//        /// completes successfully.</description> </item> </list></returns>
//        [HttpPut("")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<ActionResult> Update([FromBody] UpdateUserCommandNew cmd)
//        {
//            try
//            {
//                ResponseHttp updateCustomerResult;
//                UpdateUserCommandNewValidator validator = new();

//                updateCustomerResult = validator.Validate(new ValidationContext<UpdateUserCommandNew>(cmd));

//                if (updateCustomerResult.Status == StatusCodes.Status400BadRequest)
//                {
//                    return BadRequest(updateCustomerResult);
//                }

//                updateCustomerResult = await _mediator.Send(cmd);

//                return Ok(updateCustomerResult);
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(ex.Message);
//            }

//        }

//        /// <summary>
//        /// Deletes the resource identified by the specified ID.
//        /// </summary>
//        /// <param name="id">The unique identifier of the resource to delete.</param>
//        /// <returns>A task that represents the asynchronous operation. The task result contains  <see langword="true"/> if the
//        /// resource was successfully deleted; otherwise,  <see langword="false"/>.</returns>
//        [HttpDelete("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<ActionResult<bool>> Delete(Guid id)
//        {
//            var result = await _mediator.Send(new DeleteUserCommandNew(id));
//            return Ok(result);
//        }

//        /// <summary>
//        /// Retrieves a resource by its unique identifier.
//        /// </summary>
//        /// <remarks>This method sends a query to retrieve the resource associated with the specified
//        /// <paramref name="id"/>. If the resource is found, it is returned with an HTTP 200 OK status. Otherwise, an
//        /// appropriate HTTP error status is returned.</remarks>
//        /// <param name="id">The unique identifier of the resource to retrieve.</param>
//        /// <returns>An <see cref="ActionResult"/> containing the resource if found, or an appropriate HTTP response if not.</returns>
//        [HttpGet("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<ActionResult> Get(Guid id)
//        {
//            GetUserByIdNewQuery qr = new(id);
//            var result = await _mediator.Send(qr);

//            return Ok(result);
//        }

//        /// <summary>
//        /// Retrieves a paginated list of user items.
//        /// </summary>
//        /// <remarks>The pagination parameters allow the caller to specify which subset of the data to
//        /// retrieve. If both <paramref name="pageNumber"/> and <paramref name="pageSize"/> are null, default values are
//        /// applied.</remarks>
//        /// <param name="pageNumber">The page number to retrieve. If null, the default page is returned.</param>
//        /// <param name="pageSize">The number of items per page. If null, the default page size is used.</param>
//        /// <returns>An <see cref="ActionResult"/> containing the paginated list of User items.</returns>
//        [HttpGet("")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//        public async Task<ActionResult> Get(int? pageNumber, int? pageSize)
//        {
//            var result = await _mediator.Send(new GetAllUserNewQuery(pageNumber, pageSize));

//            return Ok(result);
//        }

//    }
//}
// API/Controllers/UsersController.cs
using Application.Features.UserFeature.Commands;
using Application.Features.UserFeature.Dtos;
using Application.Features.UserFeature.Queries;
using Application.Features.UserFeature.Validators;
using Application.Setting;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/admin/users")]
    [ApiController]
    //[Authorize(Roles = "Administrateur")] // Uncomment when authentication is implemented
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves a paginated list of users with optional filtering and sorting
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <param name="sortBy">Sort field (email, firstName, lastName, role, createdAt)</param>
        /// <param name="sortDescending">Sort in descending order</param>
        /// <param name="searchTerm">Search term for email, first name, or last name</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetUsers(
            [FromQuery] int? pageNumber = 1,
            [FromQuery] int? pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool? sortDescending = false,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetAllUsersQuery(pageNumber, pageSize, sortBy, sortDescending, searchTerm);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="createUserDto">User creation data</param>
        /// <returns>Created user</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> CreateUser([FromBody] CreateUserDTO createUserDto)
        {
            try
            {
                var command = new AddUserCommand(createUserDto);
                var validator = new AddUserCommandValidator();
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
        /// Retrieves a user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateUser(Guid id, [FromBody] UpdateUserDTO updateUserDto)
        {
            if (id != updateUserDto.Id)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = "ID in URL does not match ID in body",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            try
            {
                var command = new UpdateUserCommand(updateUserDto);
                var validator = new UpdateUserCommandValidator();
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
        /// Deletes a user (soft delete)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> DeleteUser(Guid id)
        {
            var command = new DeleteUserCommand(id);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Updates a user's role
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="roleDto">New role</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateUserRole(Guid id, [FromBody] UpdateUserRoleDTO roleDto)
        {
            var command = new UpdateUserRoleCommand(id, roleDto.Role);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Resets a user's password
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="resetPasswordDto">New password</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> ResetPassword(Guid id, [FromBody] ResetPasswordDTO resetPasswordDto)
        {
            if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword) || resetPasswordDto.NewPassword.Length < 6)
            {
                return BadRequest(new ResponseHttp
                {
                    Fail_Messages = "Password must be at least 6 characters long",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var command = new ResetUserPasswordCommand(id, resetPasswordDto.NewPassword);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }




        /// <summary>
        /// Restores a soft-deleted user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> RestoreUser(Guid id)
        {
            var command = new RestoreUserCommand(id);
            var result = await _mediator.Send(command);

            if (result.Status == StatusCodes.Status200OK)
                return Ok(result);

            if (result.Status == StatusCodes.Status404NotFound)
                return NotFound(result);

            return BadRequest(result);
        }
    }
}
