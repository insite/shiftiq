using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGradebookEnrollments : Query<int>, IGradebookEnrollmentCriteria
    {
        public Guid? GradebookId { get; set; }
        public Guid? LearnerId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PeriodId { get; set; }

        public string EnrollmentComment { get; set; }

        public int? EnrollmentRestart { get; set; }

        public DateTimeOffset? EnrollmentCompleted { get; set; }
        public DateTimeOffset? EnrollmentStarted { get; set; }
    }
}