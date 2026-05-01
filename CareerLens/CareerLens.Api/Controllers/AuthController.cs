using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Identity;
using CareerLens.Application.Features.Identity.Commands.RegisterUser;
using CareerLens.Application.Features.Identity.Dtos;
using CareerLens.Application.Features.Identity.Queries.GenerateTokens;
using CareerLens.Application.Features.Identity.Queries.GetUserInfo;
using CareerLens.Application.Features.Identity.Queries.RefreshTokens;
using CareerLens.Contracts.Requests.Identity;
using CareerLens.Application.Common.Helper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CareerLens.Api.Controllers
{
    [Route("api/auth")]
    public class AuthController(ISender sender) : ApiController
    {
        private readonly ISender _sender = sender;

        #region Register
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Register(
            [FromBody] RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var command = new RegisterUserCommand(
                FirstName: request.FirstName,
                LastName: request.LastName,
                Email: request.Email,
                Password: request.Password,
                ConfirmPassword: request.ConfirmPassword,
                Role: request.Role);

            var result = await _sender.Send(command, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : CreatedAtAction(nameof(GetMe), null, result.Value);
        }
        #endregion

        #region Login
        
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new GenerateTokenQuery(
                Email: request.Email,
                Password: request.Password);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Refresh Token
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            CancellationToken cancellationToken = default)
        {
            var query = new RefreshTokenQuery(
                RefreshToken: request.RefreshToken,
                ExpiredAccessToken: request.ExpiredAccessToken);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

        #region Get Loged In User
        
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMe(
            [FromServices] IUser currentUser,
            CancellationToken cancellationToken = default)
        {
            var userIdResult = currentUser.GetUserId();

            if (userIdResult.IsError)
                return Problem(userIdResult.Errors);

            var query = new GetUserByIdQuery(UserId: userIdResult.Value);

            var result = await _sender.Send(query, cancellationToken);

            return result.IsError
                ? Problem(result.Errors)
                : Ok(result.Value);
        }
        #endregion

    }
}
