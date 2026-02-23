using System;

namespace Shift.Contract
{
    public class CreateGradebookEnrollment
    {
        public Guid EnrollmentId { get; set; }
        public Guid GradebookId { get; set; }
        public Guid LearnerId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PeriodId { get; set; }

        public string EnrollmentComment { get; set; }

        public int EnrollmentRestart { get; set; }

        public DateTimeOffset? EnrollmentCompleted { get; set; }
        public DateTimeOffset? EnrollmentStarted { get; set; }
    }
}