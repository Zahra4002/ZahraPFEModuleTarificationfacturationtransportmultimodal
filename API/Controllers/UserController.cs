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

        /// <summary>
        /// Updates user profile with optional profile picture (FormData endpoint for file upload)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}/profile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> UpdateUserProfile(Guid id)
        {
            try
            {
                // Read form data
                var form = await Request.ReadFormAsync();
                
                // Extract fields from form
                var idStr = form["Id"].ToString();
                var firstName = form["FirstName"].ToString();
                var lastName = form["LastName"].ToString();
                var email = form["Email"].ToString();
                var phoneNumber = form["PhoneNumber"].ToString();
                var isActiveStr = form["IsActive"].ToString();

                // Validate required fields
                if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || 
                    string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "First name, last name, and email are required",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Validate ID matches
                if (!Guid.TryParse(idStr, out var parsedId) || parsedId != id)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "ID in URL does not match ID in body",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Get current user to preserve their existing role
                var getUserQuery = new GetUserByIdQuery(parsedId);
                var getUserResult = await _mediator.Send(getUserQuery);
                if (getUserResult?.Status != StatusCodes.Status200OK)
                {
                    return NotFound(new ResponseHttp
                    {
                        Fail_Messages = "User not found",
                        Status = StatusCodes.Status404NotFound
                    });
                }

                var existingUserData = getUserResult.Resultat as UserDTO;
                var currentRole = existingUserData?.Role ?? "Lecture";

                // Create update DTO with current role preserved
                var updateDto = new UpdateUserDTO
                {
                    Id = parsedId,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    IsActive = bool.TryParse(isActiveStr, out var isActive) && isActive,
                    Role = currentRole // Preserve existing role during profile update
                };

                // Handle profile picture file if provided
                var profilePictureFile = form.Files.GetFile("ProfilePicture");
                if (profilePictureFile != null && profilePictureFile.Length > 0)
                {
                    // Validate file size (max 500 MB)
                    const long maxFileSize = 500 * 1024 * 1024;
                    if (profilePictureFile.Length > maxFileSize)
                    {
                        return BadRequest(new ResponseHttp
                        {
                            Fail_Messages = $"Profile picture must be less than 500 MB (current: {(profilePictureFile.Length / (1024.0 * 1024)).ToString("F2")} MB)",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    // Validate file type
                    var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
                    if (!allowedContentTypes.Contains(profilePictureFile.ContentType.ToLower()))
                    {
                        return BadRequest(new ResponseHttp
                        {
                            Fail_Messages = $"Invalid file type. Allowed types: JPG, PNG, WebP, GIF",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    // Convert file to base64
                    using (var memoryStream = new MemoryStream())
                    {
                        await profilePictureFile.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        var base64String = Convert.ToBase64String(fileBytes);
                        updateDto.ProfilePicture = $"data:{profilePictureFile.ContentType};base64,{base64String}";
                    }
                }

                // Send update command
                var command = new UpdateUserCommand(updateDto);
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
            catch (InvalidOperationException ex)
            {
                // Request body too large
                return StatusCode(StatusCodes.Status413PayloadTooLarge, new ResponseHttp
                {
                    Fail_Messages = "Request body is too large. Maximum 100 MB allowed.",
                    Status = StatusCodes.Status413PayloadTooLarge
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseHttp
                {
                    Fail_Messages = $"Error updating profile: {ex.Message}",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
