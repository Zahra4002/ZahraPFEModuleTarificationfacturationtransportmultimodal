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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginRequest?.Email) || string.IsNullOrWhiteSpace(loginRequest?.Password))
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "Email and password are required",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

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
        [AllowAnonymous]
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
        /// Logs out the current user - requires authentication
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
        /// Changes the current user's password - requires authentication
        /// Users can only change their own password. The Email in the request must match the authenticated user's email.
        /// </summary>
        /// <param name="changePasswordRequest">Password change information with user email for verification</param>
        /// <returns>Success status</returns>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> ChangePassword([FromBody] ChangePasswordRequestDTO changePasswordRequest)
        {
            try
            {
                // Get current authenticated user's email
                var currentUserEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                var userId = GetCurrentUserId();

                // Verify user is authenticated
                if (userId == null || string.IsNullOrEmpty(currentUserEmail))
                {
                    return Unauthorized(new ResponseHttp
                    {
                        Fail_Messages = "User not authenticated",
                        Status = StatusCodes.Status401Unauthorized
                    });
                }

                // SECURITY: Verify that the user can only change their own password
                // The email in the request must match the authenticated user's email
                if (!currentUserEmail.Equals(changePasswordRequest.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid();
                }

                // Validate passwords match
                if (changePasswordRequest.NewPassword != changePasswordRequest.ConfirmNewPassword)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "New password and confirmation do not match",
                        Status = StatusCodes.Status400BadRequest
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
        /// Gets the currently authenticated user's information - requires authentication
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

        /// <summary>
        /// Initiates password reset by sending a reset code to the user's email
        /// This is an anonymous endpoint for users who forgot their password
        /// </summary>
        /// <param name="forgotPasswordRequest">User email address</param>
        /// <returns>Success status</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> ForgotPassword([FromBody] ForgotPasswordRequestDTO forgotPasswordRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(forgotPasswordRequest?.Email))
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "Email is required",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var command = new ForgotPasswordCommand(forgotPasswordRequest);
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
        /// Verifies the reset code sent to user's email
        /// This is an anonymous endpoint used before resetting password
        /// </summary>
        /// <param name="verifyResetCodeRequest">Email and reset code</param>
        /// <returns>Verification result</returns>
        [HttpPost("verify-reset-code")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> VerifyResetCode([FromBody] VerifyResetCodeRequestDTO verifyResetCodeRequest)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(verifyResetCodeRequest?.Email) || string.IsNullOrWhiteSpace(verifyResetCodeRequest?.Code))
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "Email and code are required",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var command = new VerifyResetCodeCommand(verifyResetCodeRequest);
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
        /// Resets the user's password using the reset code
        /// This is an anonymous endpoint for users who forgot their password
        /// </summary>
        /// <param name="resetPasswordRequest">Email, reset code, and new password</param>
        /// <returns>Success status</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseHttp>> ResetPassword([FromBody] ResetPasswordRequestDTO resetPasswordRequest)
        {
            try
            {
                if (resetPasswordRequest?.NewPassword != resetPasswordRequest?.ConfirmNewPassword)
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "New password and confirmation do not match",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                if (string.IsNullOrWhiteSpace(resetPasswordRequest?.Email) || string.IsNullOrWhiteSpace(resetPasswordRequest?.Code))
                {
                    return BadRequest(new ResponseHttp
                    {
                        Fail_Messages = "Email and code are required",
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                var command = new ResetPasswordCommand(resetPasswordRequest);
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
        /// Extracts the current authenticated user's ID from JWT claims
        /// </summary>
        /// <returns>User ID if authenticated, null otherwise</returns>
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