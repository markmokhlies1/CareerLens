using CareerLens.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CareerLens.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<TRequest> _logger;
        private readonly IUser _user;
        private readonly IIdentityService _identityService;

        public PerformanceBehaviour(
            ILogger<TRequest> logger,
            IUser user,
            IIdentityService identityService)
        {
            _timer = new Stopwatch();

            _logger = logger;
            _user = user;
            _identityService = identityService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            if (elapsedMilliseconds > 500)
            {
                var requestName = typeof(TRequest).Name;
                var userIdString = _user.Id ?? string.Empty;
                var userName = string.Empty;

                if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out var userId))
                {
                    userName = await _identityService.GetUserNameAsync(userId);
                }

                _logger.LogWarning(
                    "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}",
                    requestName, elapsedMilliseconds, userIdString, userName, request);
            }

            return response;
        }
    }
}
