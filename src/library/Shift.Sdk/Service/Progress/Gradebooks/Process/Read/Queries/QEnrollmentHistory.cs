using System;

namespace InSite.Application.Records.Read
{
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
