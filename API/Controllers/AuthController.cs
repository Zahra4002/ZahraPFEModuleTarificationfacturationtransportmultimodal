// API/Controllers/AuthController.cs
using Application.Features.AuthFeature.Commands;
using Application.Features.AuthFeature.Dtos;
using Application.Features.AuthFeature.Queries;
using Application.Features.AuthFeature.Validators;
using Application.Setting;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="loginRequest">Login credentials</param>
        /// <returns>JWT token and user information</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                var command = new LoginCommand(loginRequest);
                var validator = new LoginCommandValidator();
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

                return Unauthorized(result);
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
        /// Refreshes an expired JWT token
        /// </summary>
        /// <param name="refreshRequest">Refresh token</param>
        /// <returns>New JWT token and refresh token</returns>
        [HttpPost("refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> Refresh([FromBody] RefreshTokenRequestDTO refreshRequest)
        {
            try
            {
                var command = new RefreshTokenCommand(refreshRequest);
                var result = await _mediator.Send(command);

                if (result.Status == StatusCodes.Status200OK)
                    return Ok(result);

                return Unauthorized(result);
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
        /// Logs out the current user
        /// </summary>
        /// <returns>Success status</returns>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> Logout()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ResponseHttp
                    {
                        Fail_Messages = "User not authenticated",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                var command = new LogoutCommand(userId.Value);
                var result = await _mediator.Send(command);

                return Ok(result);
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
        /// Changes the current user's password
        /// </summary>
        /// <param name="changePasswordRequest">Password change information</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordRequest)
        {
            try
            {
                if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "New password and confirmation do not match",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ResponseHttp
                    {
                        Fail_Messages = "User not authenticated",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                var command = new ChangePasswordCommand(
                    userId.Value,
                    changePasswordRequest.CurrentPassword,
                    changePasswordRequest.NewPassword
                );

                var validator = new ChangePasswordCommandValidator();
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
        /// Gets the currently authenticated user's information
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> GetCurrentUser()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Unauthorized(new ResponseHttp
                    {
                        Fail_Messages = "User not authenticated",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                var query = new GetCurrentUserQuery(userId.Value);
                var result = await _mediator.Send(query);

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

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                              User.FindFirst("sub")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
    }
}