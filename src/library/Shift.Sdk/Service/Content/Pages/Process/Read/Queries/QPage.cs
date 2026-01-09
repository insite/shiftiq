using System;
using System.Collections.Generic;

namespace InSite.Application.Sites.Read
{
    public class QPage
    {
        public Guid? ParentPageIdentifier { get; set; }
        public Guid? ObjectIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid PageIdentifier { get; set; }
        public Guid? SiteIdentifier { get; set; }

        public string AuthorName { get; set; }
        public string ContentControl { get; set; }
        public string ContentLabels { get; set; }
        public string Hook { get; set; }
        public string NavigateUrl { get; set; }
        public string ObjectType { get; set; }
        public string PageIcon { get; set; }
        public string PageSlug { get; set; }
        public string PageType { get; set; }
        public string Title { get; set; }

        public bool IsAccessDenied { get; set; }
        public bool IsHidden { get; set; }
        public bool IsNewTab { get; set; }

        public int Sequence { get; set; }

        public DateTimeOffset? AuthorDate { get; set; }

        public DateTimeOffset? LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public virtual QSite Site { get; set; }
        public virtual QPage Parent { get; set; }
        public virtual ICollection<QPage> Children { get; set; } = new HashSet<QPage>();

        public QPage Clone()
        {
            return (QPage)MemberwiseClone();
        }
    }

    public class VAssessmentPage
    {
        public Guid BankIdentifier { get; set; }
        public Guid FormIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid PageIdentifier { get; set; }

        public string FormCode { get; set; }
        public string FormName { get; set; }
        public string FormPublicationStatus { get; set; }
        public string FormTitle { get; set; }
        public string PageIcon { get; set; }
        public string PageTitle { get; set; }

        public bool PageIsHidden { get; set; }

        public int FormAsset { get; set; }
        public int FormAssetVersion { get; set; }

        public string PageVisibilityHtml
            => PageIsHidden
                ? "<span class=\"badge bg-danger\">Hidden</span>"
                : "<span class=\"badge bg-success\">Visible</span>";
    }
}