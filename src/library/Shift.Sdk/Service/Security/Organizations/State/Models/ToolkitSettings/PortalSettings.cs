using System;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class PortalSettings
    {
        public bool ShowMyDashboard { get; set; }
        public bool ShowMyDashboardAfterLogin { get; set; }
        public bool NotSendWelcomeMessage { get; set; }

        public bool IsEqual(PortalSettings other)
        {
            return ShowMyDashboard == other.ShowMyDashboard
                && ShowMyDashboardAfterLogin == other.ShowMyDashboardAfterLogin
                && NotSendWelcomeMessage == other.NotSendWelcomeMessage;
        }
    }
}
