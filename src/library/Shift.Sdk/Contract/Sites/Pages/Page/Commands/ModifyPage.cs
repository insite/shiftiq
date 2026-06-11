using System;

namespace Shift.Contract
{
    public class ModifyPage
    {
        public Guid? ObjectId { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PageId { get; set; }
        public Guid? ParentPageId { get; set; }
        public Guid? SiteId { get; set; }

        public bool IsAccessDenied { get; set; }
        public bool IsHidden { get; set; }
        public bool IsNewTab { get; set; }

        public string AuthorName { get; set; }
        public string ContentControl { get; set; }
        public string ContentLabels { get; set; }
        public string Hook { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }
        public string NavigateUrl { get; set; }
        public string ObjectType { get; set; }
        public string PageIcon { get; set; }
        public string PageSlug { get; set; }
        public string PageType { get; set; }
        public string Title { get; set; }

        public int Sequence { get; set; }

        public DateTimeOffset? AuthorDate { get; set; }
        public DateTimeOffset? LastChangeTime { get; set; }
    }
}