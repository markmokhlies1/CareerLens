using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Identity.Commands.LoginUser
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<TokenResponse>>;

    public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithErrorCode("Email_Required")
                .EmailAddress().WithErrorCode("Email_Invalid");

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode("Password_Required");
        }
    }

    public class LoginCommandHandler(
        IIdentityService identityService,
        ITokenProvider tokenProvider,
        ILogger<LoginCommandHandler> logger)
        : IRequestHandler<LoginCommand, Result<TokenResponse>>
    {
        public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken ct)
        {
            var authResult = await identityService.AuthenticateAsync(request.Email, request.Password);
            if (authResult.IsError) return authResult.Errors;

            var tokenResult = await tokenProvider.GenerateJwtTokenAsync(authResult.Value, ct);
            if (tokenResult.IsError)
            {
                logger.LogError("Token generation failed: {Error}", tokenResult.TopError.Description);
                return tokenResult.Errors;
            }

            return tokenResult.Value;
        }
    }
}
