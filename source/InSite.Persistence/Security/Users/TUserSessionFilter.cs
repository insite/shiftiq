using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TUserSessionFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public string[] OrganizationPersonTypes { get; set; }

        public string UserAgent { get; set; }
        public string UserBrowser { get; set; }
        public string UserEmail { get; set; }
        public string UserHostAddress { get; set; }
        public string UserLanguage { get; set; }

        public bool? SessionIsAuthenticated { get; set; }

        public DateTimeOffset? SessionStartedSince { get; set; }
        public DateTimeOffset? SessionStartedBefore { get; set; }
    }
}
