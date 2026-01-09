using Shift.Constant;

namespace Shift.Service.Presentation;

public class ReactStartupOptions
{
    public string AdminLogoUrl { get; set; } = null!;

    public string DashboardHeadingText { get; set; } = "My Dashboard";
    public string DashboardPageUrl { get; set; } = RelativeUrl.PortalHomeUrl;

    public string HelpPageUrl { get; set; } = null!;

    public int SessionTimeoutMinutes { get; set; } = 60;
}
