using System;
using System.Collections.Generic;

using Shift.Common;

namespace InSite.Application.Sites.Read
{
    [Serializable]
    public class QPageFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? WebFolderIdentifier { get; set; }
        public Guid? WebSiteIdentifier { get; set; }
        public Guid? CourseIdentifier { get; set; }
        public Guid? ProgramIdentifier { get; set; }
        public Guid? PermissionGroupIdentifier { get; set; }

        public bool? WebSiteAssigned { get; set; }
        public bool? IsPublished { get; set; }

        public string PageSlug { get; set; }
        public string PageSlugExact { get; set; }
        public string Title { get; set; }
        public string ContentControl { get; set; }
        public string Keyword { get; set; }

        public DateTimeRange Modified { get; set; }

        public List<string> Types { get; set; } = new List<string>();

        public QPageFilter()
        {
            Modified = new DateTimeRange();
        }
    }

    [Serializable]
    public class VAssessmentPageFilter : Filter
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PageIdentifier { get; set; }
        public string FormName { get; set; }
        public string PageTitle { get; set; }
        public int? FormAsset { get; set; }
        public bool? PageIsHidden { get; set; }

        public VAssessmentPageFilter Clone()
        {
            return (VAssessmentPageFilter)MemberwiseClone();
        }
    }
}
