using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Events;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using CareerLens.Domain.DomainUsers.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequests.EventHandlers.ClaimRequestCreatedEvent
{
    public sealed class CompanyClaimRequestCreatedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<CompanyClaimRequestCreatedEventHandler> logger)
    : INotificationHandler<CompanyClaimRequestCreated>
    {
        public async Task Handle(CompanyClaimRequestCreated domainEvent, CancellationToken cancellationToken)
        {
            var admins = await context.DomainUsers
                .Where(u => u.Role == Role.Admin)
                .ToListAsync(cancellationToken);

            if (admins.Count == 0)
            {
                logger.LogWarning("No admins found in the system.");
                return;
            }

            var notifications = new List<Notification>();

            foreach (var admin in admins)
            {
                var notificationResult = Notification.Create(
                    id: Guid.NewGuid(),
                    userId: admin.Id,
                    title: "New Company Claim Request",
                    message: $"A new claim request for '{domainEvent.CompanyName}' " +
                             $"has been submitted for the role of '{domainEvent.RequestedRole}' " +
                              "and is pending your review.",
                    type: NotificationType.ClaimRequestSubmitted,
                    referenceId: domainEvent.ClaimRequestId.ToString(),
                    referenceType: nameof(CompanyClaimRequest)
                );

                if (notificationResult.IsError)
                {
                    logger.LogError(
                        "Failed to create notification for admin {AdminId}",
                        admin.Id);
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
                "Notified {Count} admins for claim request {ClaimRequestId}",
                notifications.Count,
                domainEvent.ClaimRequestId);
        }
    }
}
