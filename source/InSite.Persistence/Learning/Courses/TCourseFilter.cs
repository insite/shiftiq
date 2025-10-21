using System;

using Shift.Common;

namespace InSite.Persistence
{
    [Serializable]
    public class TCourseFilter : Filter
    {
        public Guid? CatalogIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }

        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public int? CourseAsset { get; set; }
        public bool HasGradebook { get; set; }

        public Guid? AlwaysIncludeCourseIdentifier { get; set; }

        public TCourseFilter Clone()
        {
            return (TCourseFilter)MemberwiseClone();
        }
    }
}