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

namespace CareerLens.Application.Features.Reviews.EventHandlers.ReviewRejectedEvent
{
    public sealed class ReviewRejectedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<ReviewRejectedEventHandler> logger)
    : INotificationHandler<ReviewRejected>
    {
        public async Task Handle(ReviewRejected domainEvent, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Review Rejected ",
                message: $"Your review '{domainEvent.Headline}' " +
                          "has been rejected. Please review and resubmit.",
                type: NotificationType.ReviewRejected,
                referenceId: domainEvent.ReviewId.ToString(),
                referenceType: nameof(Review)
            );

            if (notificationResult.IsError)
            {
                logger.LogError(
                    "Failed to create rejection notification for user {UserId}",
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
                "Rejection notification sent to user {UserId} for review {ReviewId}",
                domainEvent.UserId,
                domainEvent.ReviewId);
        }
    }
}
