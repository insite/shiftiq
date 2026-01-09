namespace Shift.Common
{
    public class OAuthAuthenticationResult
    {
        public bool TenantMismatch { get; set; } = false;
        public bool Authorized { get; set; } = false;
        public string EmailAddress { get; set; }
        public string BearerToken { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string PreferredLanguage { get; set; }
        public string UserPrincipalName { get; set; }
        public OAuthAuthenticationMethods AuthenticationMethod { get; set; }
        public OAuthCacheEntry CacheEntry { get; set; }
    }
}
