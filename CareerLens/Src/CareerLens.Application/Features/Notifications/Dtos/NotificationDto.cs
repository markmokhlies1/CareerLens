using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Application.Features.Notifications.Dtos
{
    public sealed record NotificationDto(Guid Id,
                                         Guid UserId,
                                         string Title,
                                         string Message,
                                         string Type,
                                         bool IsRead,
                                         string? ReferenceId,
                                         string? ReferenceType);
}
