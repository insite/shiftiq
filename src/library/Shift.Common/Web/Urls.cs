using Shift.Constant;

namespace Shift.Common
{
    public class Urls
    {
        private readonly EnvironmentModel _environment;

        private readonly string _applicationDomain;

        private readonly string _helpUrl;

        public Urls(EnvironmentModel environment, string applicationDomain, string helpUrl)
        {
            _environment = environment;

            _applicationDomain = applicationDomain;

            _helpUrl = helpUrl;
        }

        public static string AdminHomeUrl => "/ui/admin/home";

        public static string AdminReportsUrl => "/ui/admin/reporting";

        public string DeveloperHelpUrl => HelpUrl.TrimEnd('/') + "/developers";

        public string HelpUrl => _helpUrl;

        public static string HomeUrl => "/ui/home";

        public string LoginUrl => "/ui/lobby/signin";

        public string MessageEditorHelpUrl => HelpUrl.TrimEnd('/') + "/help/messages/authoring-messages/edit-contents";

        public static string PortalHomeUrl => "/ui/portal/home";

        public string RootUrl => "/";

        public static string StartImpersonation => "/ui/portal/security/impersonation/start";

        public static string StopImpersonation => "/ui/portal/security/impersonation/stop";

        public string GetApplicationUrl(string organizationCode)
        {
            var subdomain = GetSubdomain(organizationCode);

            return "https://" + subdomain + "." + _applicationDomain;
        }

        public string GetHostName(string subdomain)
        {
            if (_environment.Name == EnvironmentName.Production)
                return $"{subdomain}.{_applicationDomain}";

            return $"{_environment.Slug}-{subdomain}.{_applicationDomain}";
        }

        public string GetHomeUrl(bool isCmdsUser, bool isCmdsOrganization, bool isAdministrator)
        {
            if (isCmdsUser && isCmdsOrganization)
                return HomeUrl;

            if (isAdministrator)
                return RelativeUrl.AdminHomeUrl;

            return RelativeUrl.PortalHomeUrl;
        }

        private string GetSubdomain(string organizationCode)
        {
            return _environment.GetSubdomainPrefix() + organizationCode;
        }
    }
}