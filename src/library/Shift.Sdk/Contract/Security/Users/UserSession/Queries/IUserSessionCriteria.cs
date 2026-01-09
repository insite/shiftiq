using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserSessionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        bool? SessionIsAuthenticated { get; set; }

        string AuthenticationErrorMessage { get; set; }
        string AuthenticationErrorType { get; set; }
        string AuthenticationSource { get; set; }
        string SessionCode { get; set; }
        string UserAgent { get; set; }
        string UserBrowser { get; set; }
        string UserBrowserVersion { get; set; }
        string UserEmail { get; set; }
        string UserHostAddress { get; set; }
        string UserLanguage { get; set; }

        int? SessionMinutes { get; set; }

        DateTimeOffset? SessionStarted { get; set; }
        DateTimeOffset? SessionStopped { get; set; }
    }
}