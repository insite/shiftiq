using System;

namespace InSite.Application.Courses.Read
{
    public class VCourse
    {
        public Guid CourseIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? CatalogIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }

        public string CourseCode { get; set; }
        public string CourseHook { get; set; }
        public string CourseLabel { get; set; }
        public string CourseName { get; set; }
        public string GradebookTitle { get; set; }
        public string CatalogName { get; set; }

        public int UnitCount { get; set; }
        public int ModuleCount { get; set; }
        public int ActivityCount { get; set; }
        public int EnrollmentStarted { get; set; }
        public int EnrollmentCompleted { get; set; }
    }
}
