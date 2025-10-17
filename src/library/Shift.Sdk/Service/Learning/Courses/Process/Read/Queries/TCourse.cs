using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Application.Courses.Read
{
    public class TCourse
    {
        public Guid? CatalogIdentifier { get; set; }
        public Guid? CompletionActivityIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid? CourseMessageCompletedToAdministrator { get; set; }
        public Guid? CourseMessageCompletedToLearner { get; set; }
        public Guid? CourseMessageStalledToAdministrator { get; set; }
        public Guid? CourseMessageStalledToLearner { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid? GradebookIdentifier { get; set; }
        public Guid ModifiedBy { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? SourceIdentifier { get; set; }

        public string CourseCode { get; set; }
        public string CourseDescription { get; set; }
        public string CourseHook { get; set; }
        public string CourseIcon { get; set; }
        public string CourseImage { get; set; }
        public string CourseLabel { get; set; }
        public string CourseLevel { get; set; }
        public string CourseName { get; set; }
        public string CoursePlatform { get; set; }
        public string CourseProgram { get; set; }
        public string CourseSlug { get; set; }
        public string CourseFlagColor { get; set; }
        public string CourseFlagText { get; set; }
        public string CourseStyle { get; set; }
        public string CourseUnit { get; set; }

        public bool AllowDiscussion { get; set; }
        public bool CourseIsHidden { get; set; }
        public bool IsMultipleUnitsEnabled { get; set; }
        public bool IsProgressReportEnabled { get; set; }

        public int CourseAsset { get; set; }
        public int? CourseSequence { get; set; }
        public int? OutlineWidth { get; set; }
        public int? SendMessageStalledAfterDays { get; set; }
        public int? SendMessageStalledMaxCount { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }
        public DateTimeOffset? Closed { get; set; }

        public TModule[] GetModules() { return Units.SelectMany(m => m.Modules).ToArray(); }

        public virtual TCatalog Catalog { get; set; }
        public virtual QGradebook Gradebook { get; set; }

        public virtual ICollection<TUnit> Units { get; set; } = new HashSet<TUnit>();
        public virtual ICollection<TCourseUser> Users { get; set; } = new HashSet<TCourseUser>();
    }
}
