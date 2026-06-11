using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountGradebookEnrollments : Query<int>, IGradebookEnrollmentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? EnrollmentCompletedSince { get; set; }
        public DateTimeOffset? EnrollmentCompletedBefore { get; set; }

        public DateTimeOffset? EnrollmentStartedSince { get; set; }
        public DateTimeOffset? EnrollmentStartedBefore { get; set; }
    }
}
