using System;

namespace InSite.Persistence
{
    public class VCatalogCourse
    {
        public Guid CatalogIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string CatalogName { get; set; }
        public string CourseCategory { get; set; }
        public string CourseFlagColor { get; set; }
        public string CourseFlagText { get; set; }
        public string CourseImage { get; set; }
        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public string CourseSlug { get; set; }
        public string CourseType { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }

        public bool CatalogIsHidden { get; set; }
        public bool CourseIsHidden { get; set; }

        public DateTimeOffset CourseCreated { get; set; }
        public DateTimeOffset CourseModified { get; set; }
    }
}