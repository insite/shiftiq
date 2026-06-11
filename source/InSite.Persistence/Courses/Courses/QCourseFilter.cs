using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class QCourseFilter : Filter
    {
        public Guid? CatalogIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }

        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public string GradebookTitle { get; set; }
        public int? CourseAsset { get; set; }
        public bool? HasGradebook { get; set; }
        public bool? HasWebPage { get; set; }

        public DateTimeOffset? WebPageAuthoredSince { get; set; }
        public DateTimeOffset? WebPageAuthoredBefore { get; set; }

        public Guid? AlwaysIncludeCourseIdentifier { get; set; }

        public QCourseFilter Clone()
        {
            return (QCourseFilter)MemberwiseClone();
        }
    }
}