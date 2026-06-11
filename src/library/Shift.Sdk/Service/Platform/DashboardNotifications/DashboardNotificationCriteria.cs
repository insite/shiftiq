using System;

using Shift.Common;

namespace Shift.Sdk.Service.Platform.DashboardNotifications
{
    public class DashboardNotificationCriteria
    {
        public QueryFilter Filter { get; set; }
        
        public string Title { get; set; }
        public bool OnlyVisibleOnDashboard { get; set; }
    }
}