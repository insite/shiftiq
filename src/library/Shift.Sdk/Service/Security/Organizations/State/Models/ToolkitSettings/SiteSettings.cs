using System;

using Shift.Common;

namespace InSite.Domain.Organizations
{
    [Serializable]
    public class SiteSettings
    {
        public string PortalLogo { get; set; }
        public string PublicSiteCacheResetUrl { get; set; }

        public bool IsEqual(SiteSettings other)
        {
            return PortalLogo.NullIfEmpty() == other.PortalLogo.NullIfEmpty()
                && PublicSiteCacheResetUrl.NullIfEmpty() == other.PublicSiteCacheResetUrl.NullIfEmpty();
        }
    }
}
