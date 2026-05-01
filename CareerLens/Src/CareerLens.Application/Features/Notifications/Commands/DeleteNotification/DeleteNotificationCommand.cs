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

namespace CareerLens.Application.Features.Notifications.Commands.DeleteNotification
{
    public sealed record DeleteNotificationCommand(Guid NotificationId)
    : IRequest<Result<Deleted>>;

    public sealed class DeleteNotificationCommandValidator
        : AbstractValidator<DeleteNotificationCommand>
    {
        public DeleteNotificationCommandValidator()
        {
            RuleFor(x => x.NotificationId)
                .NotEmpty()
                .WithMessage("Notification ID is required.");
        }
    }

    public sealed class DeleteNotificationCommandHandler(IAppDbContext context, IUser currentUser)
        : IRequestHandler<DeleteNotificationCommand, Result<Deleted>>
    {
        private readonly IAppDbContext _context = context;
        private readonly IUser _currentUser = currentUser;

        public async Task<Result<Deleted>> Handle(
            DeleteNotificationCommand request,
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

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Deleted;
        }
    }
}
