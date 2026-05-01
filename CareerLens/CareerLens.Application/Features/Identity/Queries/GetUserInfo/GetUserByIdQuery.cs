using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Identity.Dtos;
using CareerLens.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Identity.Queries.GetUserInfo
{
    public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<AppUserDto>>;

    public class GetUserByIdQueryHandler(ILogger<GetUserByIdQueryHandler> logger, IIdentityService identityService)
    : IRequestHandler<GetUserByIdQuery, Result<AppUserDto>>
    {
        private readonly ILogger<GetUserByIdQueryHandler> _logger = logger;
        private readonly IIdentityService _identityService = identityService;

        public async Task<Result<AppUserDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
        {
            var getUserByIdResult = await _identityService.GetUserByIdAsync(request.UserId);

            if (getUserByIdResult.IsError)
            {
                _logger.LogError("User with Id { UserId }{ErrorDetails}", request.UserId, getUserByIdResult.TopError.Description);

                return getUserByIdResult.Errors;
            }

            return getUserByIdResult.Value;
        }
    }
}
