using System;

namespace Shift.Contract
{
    public class ModifyGradebookEnrollment
    {
        public Guid EnrollmentIdentifier { get; set; }
        public Guid GradebookIdentifier { get; set; }
        public Guid LearnerIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }

        public string EnrollmentComment { get; set; }

        public int EnrollmentRestart { get; set; }

        public DateTimeOffset? EnrollmentCompleted { get; set; }
        public DateTimeOffset? EnrollmentStarted { get; set; }
    }
}