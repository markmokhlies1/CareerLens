using CareerLens.Application.Features.Notifications.Dtos;
using CareerLens.Domain.Notifications;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Notifications.Mappers
{
    public static class NotificationMappings
    {
        public static NotificationDto ToDto(this Notification notification)
        {
            return new NotificationDto(
            Id: notification.Id,
            UserId: notification.UserId,
            Title: notification.Title!,
            Message: notification.Message!,
            Type: notification.Type.ToString(),
            IsRead: notification.IsRead,
            ReferenceId: notification.ReferenceId,
            ReferenceType: notification.ReferenceType
            );
        }
    }
}
