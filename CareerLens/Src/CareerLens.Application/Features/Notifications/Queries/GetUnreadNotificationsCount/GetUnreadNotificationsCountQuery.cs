using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Notifications.Queries.GetUnreadNotificationsCount
{
    public sealed record GetUnreadNotificationsCountQuery()
    : IRequest<Result<int>>;

    public sealed class GetUnreadNotificationsCountQueryHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<GetUnreadNotificationsCountQuery, Result<int>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<int>> Handle(
            GetUnreadNotificationsCountQuery request,
            CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var count = await _context.Notifications
                .AsNoTracking()
                .CountAsync(n => n.UserId == userId
                              && n.IsRead == false,
                            cancellationToken);

            return count;
        }
    }
}
