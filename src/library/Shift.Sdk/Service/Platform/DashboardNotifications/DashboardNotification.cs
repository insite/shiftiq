using System;

using Newtonsoft.Json;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public enum DashboardNotificationType { None, PlatformUpdate, ReleaseNotes, Other }

    public class DashboardNotification
    {
        public Guid NotificationId { get; set; }
        public DashboardNotificationType Type { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
    }
}