using System;

namespace Shift.Common
{
    public class OAuthCacheEntry
    {
        public const int LifetimeSeconds = 5 * 60;

        public Guid OrganizationId { get; set; }
        public string Url { get; set; }
        public OAuthMethod Method { get; set; }
    }
}
