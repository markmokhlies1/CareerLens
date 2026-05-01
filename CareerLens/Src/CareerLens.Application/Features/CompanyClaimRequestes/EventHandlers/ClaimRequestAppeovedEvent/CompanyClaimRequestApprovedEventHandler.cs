using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Companies.CompanyClaimRequests;
using CareerLens.Domain.Companies.CompanyClaimRequests.Events;
using CareerLens.Domain.Companies.CompanyMembers;
using CareerLens.Domain.Notifications;
using CareerLens.Domain.Notifications.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.CompanyClaimRequestes.EventHandlers.ClaimRequestApprovedEvent
{
    public sealed class CompanyClaimRequestApprovedEventHandler(IAppDbContext context, IRealTimeNotificationService realTimeNotificationService, ILogger<CompanyClaimRequestApprovedEventHandler> logger)
    : INotificationHandler<CompanyClaimRequestApproved>
    {
        public async Task Handle(CompanyClaimRequestApproved domainEvent, CancellationToken cancellationToken)
        {
            var companyMemberResult = CompanyMember.Create(
                id : Guid.NewGuid(),
                companyId: domainEvent.CompanyId,
                userId: domainEvent.UserId,
                role: domainEvent.RequestedRole
            );

            if (companyMemberResult.IsError)
            {
                logger.LogError(
                    "Failed to create company member for user {UserId} in company {CompanyId}",
                    domainEvent.UserId,
                    domainEvent.CompanyId);
                return;
            }

            await context.CompanyMembers.AddAsync(companyMemberResult.Value, cancellationToken);

            

            var notificationResult = Notification.Create(
                id: Guid.NewGuid(),
                userId: domainEvent.UserId,
                title: "Claim Request Approved",
                message: $"Your claim request for '{domainEvent.CompanyName}' " +
                         $"has been approved. You are now a '{domainEvent.RequestedRole}' " +
                          "member of the company.",
                type: NotificationType.ClaimRequestApproved,
                referenceId: domainEvent.ClaimRequestId.ToString(),
                referenceType: nameof(CompanyClaimRequest)
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
                "User {UserId} added as {Role} to company {CompanyId} and notified.",
                domainEvent.UserId,
                domainEvent.RequestedRole,
                domainEvent.CompanyId);
        }
    }
}
