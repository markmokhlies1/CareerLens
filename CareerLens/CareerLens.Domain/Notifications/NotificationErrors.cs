using CareerLens.Domain.Common.Results;

namespace CareerLens.Domain.Notifications
{
    public static class NotificationErrors
    {
        public static  Error IdRequired =>
            Error.Validation("Notification.IdRequired", "Notification ID is required.");

        public static  Error UserIdRequired => 
            Error.Validation("Notification.UserIdRequired","User ID is required.");

        public static  Error TitleRequired => 
            Error.Validation("Notification.TitleRequired", "Notification title is required.");

        public static  Error MessageRequired =>
            Error.Validation("Notification.MessageRequired", "Notification message is required.");

        public static  Error TypeInvalid =>
            Error.Validation("Notification.TypeInvalid", "Invalid notification type.");

        public static  Error AlreadyRead => 
            Error.Conflict("Notification.AlreadyRead","Notification has already been marked as read.");
    }
}
