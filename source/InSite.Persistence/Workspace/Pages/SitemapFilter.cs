using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class SitemapFilter : Filter
    {
        public Guid? FolderIdentifier { get; set; }
        public Guid? PageIdentifier { get; set; }
        public Guid? SiteIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string PageType { get; set; }
    }
}
