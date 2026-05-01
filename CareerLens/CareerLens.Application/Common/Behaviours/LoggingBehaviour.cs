using CareerLens.Application.Common.Interfaces;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Common.Behaviours
{
    public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
    {
        private readonly ILogger _logger = logger;
        private readonly IUser _user = user;
        private readonly IIdentityService _identityService = identityService;

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userIdString = _user.Id ?? string.Empty;
            string? userName = string.Empty;

            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogInformation(
                "Request: {Name} {@UserId} {@UserName} {@Request}", requestName, userIdString, userName, request);
        }
    }
}
