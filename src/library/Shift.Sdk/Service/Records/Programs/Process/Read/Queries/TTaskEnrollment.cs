using System;

namespace InSite.Application.Records.Read
{
    [Serializable]
    public class TTaskEnrollment
    {
        public Guid EnrollmentIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid TaskIdentifier { get; set; }
        public Guid ObjectIdentifier { get; set; }

        public DateTimeOffset? ProgressStarted { get; set; }
        public DateTimeOffset? ProgressCompleted { get; set; }

        public virtual TTask Task { get; set; }
    }
}
