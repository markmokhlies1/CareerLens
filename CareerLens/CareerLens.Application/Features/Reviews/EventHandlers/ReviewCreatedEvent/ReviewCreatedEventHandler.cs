using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Companies.CompanyMembers.Enums;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using CareerLens.Domain.Reviews;
using CareerLens.Domain.Reviews.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Reviews.EventHandlers.ReviewCreatedEvent
{
    public sealed class ReviewCreatedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<ReviewCreatedEventHandler> logger)
    : INotificationHandler<ReviewCreated>
    {
        public async Task Handle(
            ReviewCreated domainEvent,
            CancellationToken cancellationToken)
        {
            var moderators = await context.CompanyMembers
                .Where(cm => cm.CompanyId == domainEvent.CompanyId
                          && cm.Role == CompanyMemberRole.Moderator)
                .ToListAsync(cancellationToken);

            if (moderators.Count == 0)
            {
                logger.LogWarning(
                    "No moderators found for company {CompanyId}",
                    domainEvent.CompanyId);
                return;
            }

            var notifications = new List<Notification>();

            foreach (var moderator in moderators)
            {
                var notificationResult = Notification.Create(
                    id: Guid.NewGuid(),
                    userId: moderator.UserId,
                    title: "New Review Submitted",
                    message: $"A new review '{domainEvent.Headline}' " +
                              "has been submitted and is pending your review.",
                    type: NotificationType.ReviewSubmitted,
                    referenceId: domainEvent.ReviewId.ToString(),
                    referenceType: nameof(Review)
                );

                if (notificationResult.IsError)
                {
                    logger.LogError(
                        "Failed to create notification for moderator {ModeratorId}",
                        moderator.UserId);
                    continue;
                }

                notifications.Add(notificationResult.Value);
            }

            if (notifications.Count == 0) return;

            await context.Notifications.AddRangeAsync(notifications, cancellationToken);

            foreach (var notification in notifications)
            {
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
                    notification.UserId,
                    dto);
            }

            logger.LogInformation(
                "Notified {Count} moderators for review {ReviewId}",
                notifications.Count,
                domainEvent.ReviewId);
        }
    }
}
