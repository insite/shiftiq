using System;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public class ActiveNotification
    {
        public Guid NotificationId { get; set; }
        public DashboardNotificationType Type { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public DateTimeOffset Modified { get; set; }
    }
}