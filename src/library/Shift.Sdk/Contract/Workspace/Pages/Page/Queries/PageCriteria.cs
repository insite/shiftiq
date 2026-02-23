using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPageCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? ObjectId { get; set; }
        Guid? ParentPageId { get; set; }
        Guid? SiteId { get; set; }

        bool? IsAccessDenied { get; set; }
        bool? IsHidden { get; set; }
        bool? IsNewTab { get; set; }

        string AuthorName { get; set; }
        string ContentControl { get; set; }
        string ContentLabels { get; set; }
        string Hook { get; set; }
        string LastChangeType { get; set; }
        string LastChangeUser { get; set; }
        string NavigateUrl { get; set; }
        string ObjectType { get; set; }
        string PageIcon { get; set; }
        string PageSlug { get; set; }
        string PageType { get; set; }
        string Title { get; set; }

        int? Sequence { get; set; }

        DateTimeOffset? AuthorDate { get; set; }
        DateTimeOffset? LastChangeTime { get; set; }

        bool? IsNullNavigateUrl { get; set; }
        string ParentPageSlug { get; set; }
        string ParentPageType { get; set; }
        string SiteDomain { get; set; }
    }
}