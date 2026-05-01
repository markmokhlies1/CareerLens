using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.EventHandlers.ReviewApprovedEvent
{
    public sealed class ReviewApprovedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<ReviewApprovedEventHandler> logger)
    : INotificationHandler<ReviewApproved>
    {
        public async Task Handle(ReviewApproved domainEvent, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Review Approved ",
                message: $"Your review '{domainEvent.Headline}' " +
                          "has been approved and is now publicly visible.",
                type: NotificationType.ReviewApproved,
                referenceId: domainEvent.ReviewId.ToString(),
                referenceType: nameof(Review)
            );

            if (notificationResult.IsError)
            {
                logger.LogError(
                    "Failed to create approval notification for user {UserId}",
                    domainEvent.UserId);
                return;
            }

            var notification = notificationResult.Value;

            await context.Notifications.AddAsync(notification, cancellationToken);

            var dto = new NotificationDto(
                Id: notification.Id,
                UserId: notification.UserId,
                Title: notification.Title!,
                Message: notification.Message!,
                Type: notification.Type.ToString(),
                IsRead: notification.IsRead,
                ReferenceId: notification.ReferenceId,
                ReferenceType: notification.ReferenceType
            );

            await realTimeNotificationService.SendNotificationAsync(
                domainEvent.UserId,
                dto);

            logger.LogInformation(
                "Approval notification sent to user {UserId} for review {ReviewId}",
                domainEvent.UserId,
                domainEvent.ReviewId);
        }
    }
}
