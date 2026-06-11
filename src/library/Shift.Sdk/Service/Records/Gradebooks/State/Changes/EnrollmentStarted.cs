using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Records
{
    public class EnrollmentStarted : Change
    {
        public EnrollmentStarted(Guid enrollment, Guid learner, Guid? period, DateTimeOffset? started, string comment)
        {
            Enrollment = enrollment;
            Learner = learner;
            Period = period;
            Started = started;
            Comment = comment;
        }

        public Guid Enrollment { get; set; }
        public Guid Learner { get; set; }
        public Guid? Period { get; set; }
        public DateTimeOffset? Started { get; set; }
        public string Comment { get; set; }
    }

    public class EnrollmentRestarted : Change
    {
        public EnrollmentRestarted(Guid learner, DateTimeOffset? restarted)
        {
            Learner = learner;
            Restarted = restarted;
        }

        public Guid Learner { get; set; }
        public DateTimeOffset? Restarted { get; set; }
    }

    public class EnrollmentCompleted : Change
    {
        public EnrollmentCompleted(Guid enrollment, Guid learner, DateTimeOffset? completed)
        {
            Enrollment = enrollment;
            Learner = learner;
            Completed = completed;
        }

        public Guid Enrollment { get; set; }
        public Guid Learner { get; set; }
        public DateTimeOffset? Completed { get; set; }
    }
}