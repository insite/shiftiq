using System;
using System.Collections.Generic;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public class DashboardNotificationList
    {
        public DashboardNotification[] Notifications { get; set; }
        public Dictionary<Guid, bool> VisibleOnDashboard { get; set; }
        public int TotalCount { get; set; }
    }
}