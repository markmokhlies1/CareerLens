using CareerLens.Application.Common.Errors;
using CareerLens.Application.Common.Helper;
using CareerLens.Application.Common.Interfaces;
using CareerLens.Domain.Common.Results;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Notifications.Commands.MarkNotificationAsRead
{
    public sealed record MarkNotificationAsReadCommand(Guid NotificationId)
    : IRequest<Result<Updated>>;

    public sealed class MarkNotificationAsReadCommandValidator
        : AbstractValidator<MarkNotificationAsReadCommand>
    {
        public MarkNotificationAsReadCommandValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty()
                .WithMessage("Notification ID is required.");
        }
    }

    public sealed class MarkNotificationAsReadCommandHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<MarkNotificationAsReadCommand, Result<Updated>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<Updated>> Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            var userIdResult = _currentUser.GetUserId();
            if (userIdResult.IsError)
            {
                return userIdResult.Errors;
            }
            var userId = userIdResult.Value;

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == request.NotificationId, cancellationToken);

            if (notification is null)
            {
                return ApplicationErrors.NotificationNotFound;
            }

            if (notification.UserId != userId)
            {
                return ApplicationErrors.NotNotificationOwner;
            }

            var markResult = notification.MarkAsRead();

            if (markResult.IsError)
            {
                return markResult.Errors;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Updated;
        }
    }

}
