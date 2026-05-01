using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using CareerLens.Domain.Salaries;
using CareerLens.Domain.Salaries.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Salaries.EventHandlers.SalaryRejectedEvent
{
    public sealed class SalaryRejectedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<SalaryRejectedEventHandler> logger)
    : INotificationHandler<SalaryRejected>
    {
        public async Task Handle(SalaryRejected domainEvent, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Salary Rejected ",
                message: $"Your salary for '{domainEvent.JobTitle}' " +
                          "has been rejected. Please review and resubmit.",
                type: NotificationType.SalaryRejected,
                referenceId: domainEvent.SalaryId.ToString(),
                referenceType: nameof(Salary)
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
                "Rejection notification sent to user {UserId} for salary {SalaryId}",
                domainEvent.UserId,
                domainEvent.SalaryId);
        }
    }
}
