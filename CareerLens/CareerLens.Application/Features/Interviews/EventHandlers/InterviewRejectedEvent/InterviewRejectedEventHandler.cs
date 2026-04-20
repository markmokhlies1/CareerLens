using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Interviews;
using CareerLens.Domain.Interviews.Events;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Interviews.EventHandlers.InterviewRejectedEvent
{
    public sealed class InterviewRejectedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<InterviewRejectedEventHandler> logger)
    : INotificationHandler<InterviewRejected>
    {
        public async Task Handle(InterviewRejected domainEvent, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Interview Rejected ",
                message: $"Your interview for '{domainEvent.JobTitle}' " +
                          "has been rejected. Please review and resubmit.",
                type: NotificationType.InterviewRejected,
                referenceId: domainEvent.InterviewId.ToString(),
                referenceType: nameof(Interview)
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

            await realTimeNotificationService.SendNotificationAsync(domainEvent.UserId,dto);

            logger.LogInformation(
                "Rejection notification sent to user {UserId} for interview {InterviewId}",
                domainEvent.UserId,
                domainEvent.InterviewId);
        }
    }
}
