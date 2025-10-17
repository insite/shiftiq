using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Records.Read
{
    public class QEnrollment
    {
        public Guid EnrollmentIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }

        public string EnrollmentComment { get; set; }

        public int EnrollmentRestart { get; set; }

        public DateTimeOffset? EnrollmentCompleted { get; set; }
        public DateTimeOffset? EnrollmentStarted { get; set; }

        public virtual QGradebook Gradebook { get; set; }
        public virtual VUser Learner { get; set; }
        public virtual QPeriod Period { get; set; }
    }

    public class QEnrollmentHistory
    {
        public Guid AggregateIdentifier { get; set; }
        public Guid ChangeBy { get; set; }
        public Guid EnrollmentIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }

        public string EnrollmentType { get; set; }

        public int AggregateVersion { get; set; }

        public DateTimeOffset ChangeTime { get; set; }
        public DateTimeOffset? EnrollmentTime { get; set; }
    }
}
