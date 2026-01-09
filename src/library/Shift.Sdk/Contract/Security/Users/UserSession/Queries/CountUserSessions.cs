using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountUserSessions : Query<int>, IUserSessionCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? UserIdentifier { get; set; }

        public bool? SessionIsAuthenticated { get; set; }

        public string AuthenticationErrorMessage { get; set; }
        public string AuthenticationErrorType { get; set; }
        public string AuthenticationSource { get; set; }
        public string SessionCode { get; set; }
        public string UserAgent { get; set; }
        public string UserBrowser { get; set; }
        public string UserBrowserVersion { get; set; }
        public string UserEmail { get; set; }
        public string UserHostAddress { get; set; }
        public string UserLanguage { get; set; }

        public int? SessionMinutes { get; set; }

        public DateTimeOffset? SessionStarted { get; set; }
        public DateTimeOffset? SessionStopped { get; set; }
    }
}