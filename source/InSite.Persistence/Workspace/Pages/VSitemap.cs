using System;

namespace InSite.Persistence
{
    public class VSitemap
    {
        public Guid? FolderIdentifier { get; set; }
        public Guid PageIdentifier { get; set; }
        public Guid SiteIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string PageSlug { get; set; }
        public string PageSlugIndented { get; set; }
        public string PageTitle { get; set; }
        public string PageTitleIndented { get; set; }
        public string PageType { get; set; }

        public bool PageIsHidden { get; set; }

        public string PathSequence { get; set; }
        public string PathUrl { get; set; }
        public string SiteDomain { get; set; }
    }
}
