using System;

namespace InSite.Application.Records.Read
{
    public class EnrollmentForPeriodGrid
    {
        public Guid LearnerIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
        public string UserEmailAlternate { get; set; }
        public string PeriodName { get; set; }
        public DateTimeOffset? PeriodStart { get; set; }
        public DateTimeOffset? PeriodEnd { get; set; }
        public DateTimeOffset? Graded { get; set; }
    }
}
