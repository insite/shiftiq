using System;

namespace Shift.Contract
{
    public class CourseMatch
    {
        public Guid OrganizationId { get; set; }

        public Guid CourseId { get; set; }

        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string CourseLabel { get; set; }
        public string CourseHook { get; set; }
        public bool CourseIsHidden { get; set; }

        public Guid? CatalogId { get; set; }
        public string CatalogName { get; set; }
        public bool? CatalogIsHidden { get; set; }

        public string PublicationStatus { get; set; }
        public DateTimeOffset? PublicationDate { get; set; }
        public string PublicationAuthor { get; set; }
        public Guid? PublicationPageId { get; set; }

        public int? UnitCount { get; set; }
        public int? ModuleCount { get; set; }
        public int? ActivityCount { get; set; }

        public int EnrollmentCount { get; set; }
        public int EnrollmentCountStarted { get; set; }
        public int EnrollmentCountCompleted { get; set; }

        public Guid? GradebookId { get; set; }
        public string GradebookTitle { get; set; }
    }

}