using CareerLens.Domain.Common;
using CareerLens.Domain.Common.Results;
using CareerLens.Domain.Notifications.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerLens.Domain.Notifications
{
    public sealed class Notification : AuditableEntity
    {
        public Guid UserId { get; private set; }
        public string? Title { get; private set; }
        public string? Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public string? ReferenceId { get; private set; }
        public string? ReferenceType { get; private set; }

        private Notification() { }

        private Notification(
        Guid id,
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        string? referenceType = null) : base(id)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
            ReferenceId = referenceId;
            ReferenceType = referenceType;
        }
        public static Result<Notification> Create(
        Guid id,
        Guid userId,
        string title,
        string message,
        NotificationType type,
        string? referenceId = null,
        string? referenceType = null)
        {
            if (id == Guid.Empty)
            {
                return NotificationErrors.IdRequired;
            }

            if (userId == Guid.Empty)
            {
                return NotificationErrors.UserIdRequired;
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                return NotificationErrors.TitleRequired;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                return NotificationErrors.MessageRequired;
            }

            if (!Enum.IsDefined(type))
            {
                return NotificationErrors.TypeInvalid;
            }

            return new Notification(id, userId, title, message, type, referenceId, referenceType);
        }
        public Result<Updated> MarkAsRead()
        {
            if (IsRead)
            {
                return NotificationErrors.AlreadyRead;
            }

            IsRead = true;
            return Result.Updated;
        }
    }
}
