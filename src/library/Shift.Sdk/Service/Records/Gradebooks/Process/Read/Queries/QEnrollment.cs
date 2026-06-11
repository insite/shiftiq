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
}
