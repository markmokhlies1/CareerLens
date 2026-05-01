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

namespace CareerLens.Application.Features.Notifications.Commands.MarkAllNotificationsAsRead
{
    public sealed record MarkAllNotificationsAsReadCommand()
    : IRequest<Result<Updated>>;

    public sealed class MarkAllNotificationsAsReadCommandHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<MarkAllNotificationsAsReadCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<Updated>> Handle(
            MarkAllNotificationsAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId
                         && n.IsRead == false)
                .ToListAsync(cancellationToken);

            if (unreadNotifications.Count == 0)
            {
                return Result.Updated;
            }

            foreach (var notification in unreadNotifications)
            {
                notification.MarkAsRead();
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }
}
