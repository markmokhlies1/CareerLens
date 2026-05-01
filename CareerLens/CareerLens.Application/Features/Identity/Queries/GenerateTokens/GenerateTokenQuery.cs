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

namespace CareerLens.Application.Features.Identity.Queries.GenerateTokens
{
    public record GenerateTokenQuery(string Email, string Password) : IRequest<Result<TokenResponse>>;

    public sealed class GenerateTokenQueryValidator : AbstractValidator<GenerateTokenQuery>
    {
        public GenerateTokenQueryValidator()
        {
            RuleFor(request => request.Email)
                .NotNull().NotEmpty()
                .WithErrorCode("Email_Null_Or_Empty")
                .WithMessage("Email cannot be null or empty");

            RuleFor(request => request.Password)
                .NotNull().NotEmpty()
                .WithErrorCode("Password_Null_Or_Empty")
                .WithMessage("Password cannot be null or empty.");
        }
    }

    public class GenerateTokenQueryHandler(ILogger<GenerateTokenQueryHandler> logger, IIdentityService identityService, ITokenProvider tokenProvider)
    : IRequestHandler<GenerateTokenQuery, Result<TokenResponse>>
    {
        private readonly ILogger<GenerateTokenQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;
        private readonly ITokenProvider _tokenProvider = tokenProvider;

        public async Task<Result<TokenResponse>> Handle(GenerateTokenQuery query, CancellationToken ct)
        {
            var userResponse = await _identityService.AuthenticateAsync(query.Email, query.Password);

            if (userResponse.IsError)
            {
                return userResponse.Errors;
            }

            var generateTokenResult = await _tokenProvider.GenerateJwtTokenAsync(userResponse.Value, ct);

            if (generateTokenResult.IsError)
            {
                _logger.LogError("Generate token error occurred: {ErrorDescription}", generateTokenResult.TopError.Description);

                return generateTokenResult.Errors;
            }

            return generateTokenResult.Value;
        }
    }
}
