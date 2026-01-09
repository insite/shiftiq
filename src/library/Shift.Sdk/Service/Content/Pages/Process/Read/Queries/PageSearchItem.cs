using System;

namespace InSite.Application.Sites.Read
{
    public class PageSearchItem
    {
        public Guid PageIdentifier { get; set; }
        public Guid? ParentPageIdentifier { get; set; }
        public Guid? SiteIdentifier { get; set; }
        public string PageType { get; set; }
        public string PageSlug { get; set; }
        public string PageHook { get; set; }
        public string PageTitle { get; set; }
        public string Scope { get; set; }
        public string SiteTitle { get; set; }
        public string SiteName { get; set; }
        public string ContentControl { get; set; }
        public string GroupPermissions { get; set; }
        public int ChildrenCount { get; set; }
        public string PublicationStatus { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
        public string LastChangeUser { get; set; }
    }
}
