using System;

namespace Shift.Common
{
    public class OAuthCacheEntry
    {
        public Guid TenantId { get; set; }
        public string URL { get; set; }
        public OAuthAuthenticationMethods Method { get; set; }
    }
}
