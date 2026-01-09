namespace Shift.Api;

public partial class CookieController
{
    internal class IntrospectionReport
    {
        public IntrospectionReport()
        {
            Authentication = new AuthenticationDetail();
            Authorization = new AuthorizationDetail();
        }

        internal class AuthenticationDetail
        {
            public string Scheme { get; set; } = null!;
            public string Status { get; set; } = null!;
            public CookieDetail Cookie { get; set; } = new CookieDetail();
        }

        internal class AuthorizationDetail
        {
            public string Policy { get; set; } = null!;
            public string Status { get; set; } = null!;
        }

        internal class CookieDetail
        {
            public string Decoded { get; set; } = null!;
            public CookieToken Deserialized { get; set; } = null!;
        }

        public AuthenticationDetail Authentication { get; set; }
        public AuthorizationDetail Authorization { get; set; }
        public IRequestCookieCollection? Cookies { get; set; }
    }
}