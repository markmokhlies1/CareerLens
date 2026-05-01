using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Events;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequestes.EventHandlers.ClaimRequestRejectedEvent
{
    public sealed class CompanyClaimRequestRejectedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<CompanyClaimRequestRejectedEventHandler> logger)
    : INotificationHandler<CompanyClaimRequestRejected>
    {
        public async Task Handle(CompanyClaimRequestRejected domainEvent, CancellationToken cancellationToken)
        {
            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Claim Request Rejected",
                message: $"Your claim request for '{domainEvent.CompanyName}' " +
                         $"as '{domainEvent.RequestedRole}' " +
                          "has been rejected. Please contact support for more information.",
                type: NotificationType.ClaimRequestRejected,
                referenceId: domainEvent.ClaimRequestId.ToString(),
                referenceType: nameof(CompanyClaimRequest)
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
                "Rejection notification sent to user {UserId} for claim request {ClaimRequestId}",
                domainEvent.UserId,
                domainEvent.ClaimRequestId);
        }
    }
}
