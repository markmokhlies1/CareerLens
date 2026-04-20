using CareerLens.Application.Common.Interfaces;
using CareerLens.Application.Features.Notifications.Dtos;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CareerLens.Infrastructure.RealTime
{
    public sealed class NotificationHub(ILogger<NotificationHub> logger) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

                logger.LogInformation(
                    "User {UserId} connected to NotificationHub",
                    userId);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId,
                    $"user_{userId}");

                logger.LogInformation(
                    "User {UserId} disconnected from NotificationHub",
                    userId);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }

    public sealed class RealTimeNotificationService(IHubContext<NotificationHub> hubContext, ILogger<RealTimeNotificationService> logger)
    : IRealTimeNotificationService
    {
        public async Task SendNotificationAsync(Guid userId, NotificationDto notification)
        {
            try
            {
                await hubContext.Clients
                    .Group($"user_{userId}")
                    .SendAsync("ReceiveNotification", notification);

                logger.LogInformation(
                    "Real-time notification sent to user {UserId}",
                    userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Failed to send real-time notification to user {UserId}",
                    userId);
            }
        }
    }
}
