namespace Shift.Service.Security;

public partial class UserSessionEntity
{
    public Guid OrganizationIdentifier { get; set; }
    public Guid SessionIdentifier { get; set; }
    public Guid UserIdentifier { get; set; }

    public bool SessionIsAuthenticated { get; set; }

    public string? AuthenticationErrorMessage { get; set; }
    public string? AuthenticationErrorType { get; set; }
    public string AuthenticationSource { get; set; } = null!;
    public string SessionCode { get; set; } = null!;
    public string? UserAgent { get; set; }
    public string UserBrowser { get; set; } = null!;
    public string? UserBrowserVersion { get; set; }
    public string UserEmail { get; set; } = null!;
    public string UserHostAddress { get; set; } = null!;
    public string UserLanguage { get; set; } = null!;

    public int? SessionMinutes { get; set; }

    public DateTimeOffset SessionStarted { get; set; }
    public DateTimeOffset? SessionStopped { get; set; }
}