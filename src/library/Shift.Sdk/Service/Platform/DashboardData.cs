using System;

using Shift.Sdk.Service.Platform.DashboardNotifications;

namespace Shift.Sdk.Service.Platform
{
    public class DashboardData
    {
        public ActiveNotification[] ActiveNotifications { get; set; }
        public bool HideMyDashboard { get; set; }
        public DashboardCounts Counts { get; set; }
    }
}