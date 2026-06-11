namespace Shift.Common
{
    public class OAuthRedirectUrl
    {
        private readonly EnvironmentModel _environment;
        private readonly string _enterprise;
        private readonly string _domain;
        private readonly string _url;

        public OAuthRedirectUrl(EnvironmentModel environment, string enterprise, string domain, string url)
        {
            _environment = environment;
            _enterprise = enterprise;
            _domain = domain;
            _url = url;
        }

        public string Get()
        {
            return UrlHelper.GetAbsoluteUrl(_domain, _environment, _url, _enterprise);
        }
    }
}
