using System;
using System.Collections.Generic;

using InSite.Application.Courses.Read;
using InSite.Application.Events.Read;
using InSite.Application.Quizzes.Read;
using InSite.Application.Standards.Read;

namespace InSite.Application.Records.Read
{
    public class QGradebook
    {
        public Guid? AchievementIdentifier { get; set; }
        public Guid? EventIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid? FrameworkIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }

        public string GradebookTitle { get; set; }
        public string GradebookType { get; set; }
        public string Reference { get; set; }

        public bool IsLocked { get; set; }

        public DateTimeOffset GradebookCreated { get; set; }

        public DateTimeOffset LastChangeTime { get; set; }
        public string LastChangeType { get; set; }
        public string LastChangeUser { get; set; }

        public virtual QEvent Event { get; set; }
        public virtual QAchievement Achievement { get; set; }
        public virtual VFramework Framework { get; set; }
        public virtual QPeriod Period { get; set; }

        public virtual ICollection<QGradeItemCompetency> Competencies { get; set; } = new HashSet<QGradeItemCompetency>();
        public virtual ICollection<QCourse> Courses { get; set; } = new HashSet<QCourse>();
        public virtual ICollection<QGradeItem> Items { get; set; } = new HashSet<QGradeItem>();
        public virtual ICollection<QEnrollment> Enrollments { get; set; } = new HashSet<QEnrollment>();
        public virtual ICollection<QProgress> Progresses { get; set; } = new HashSet<QProgress>();
        public virtual ICollection<QGradebookCompetencyValidation> Validations { get; set; } = new HashSet<QGradebookCompetencyValidation>();
        public virtual ICollection<TQuiz> Quizzes { get; set; } = new HashSet<TQuiz>();
        public virtual ICollection<QGradebookEvent> GradebookEvents { get; set; } = new HashSet<QGradebookEvent>();
    }
}