using System;

namespace InSite.Persistence
{
    public class VWebPageHierarchy
    {
        public string PathIndent { get; set; }
        public string PathIdentifier { get; set; }
        public string PathUrl { get; set; }
        public string PathSequence { get; set; }
        public string PageSlug { get; set; }
        public bool SiteIsPortal { get; set; }
        public string WebPageTitle { get; set; }
        public string WebPageType { get; set; }
        public string WebSiteDomain { get; set; }

        public Guid? ParentWebPageIdentifier { get; set; }
        public int PathDepth { get; set; }
        public Guid RootWebPageIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid WebPageIdentifier { get; set; }
        public Guid WebSiteIdentifier { get; set; }
    }
}
