using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGradebookEnrollments : Query<int>, IGradebookEnrollmentCriteria
    {
        public Guid? GradebookIdentifier { get; set; }
        public Guid? LearnerIdentifier { get; set; }
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? PeriodIdentifier { get; set; }

        public string EnrollmentComment { get; set; }

        public int? EnrollmentRestart { get; set; }

        public DateTimeOffset? EnrollmentCompleted { get; set; }
        public DateTimeOffset? EnrollmentStarted { get; set; }
    }
}