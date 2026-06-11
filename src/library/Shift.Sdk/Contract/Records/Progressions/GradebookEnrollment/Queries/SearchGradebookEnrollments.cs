using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchGradebookEnrollments : Query<IEnumerable<GradebookEnrollmentMatch>>, IGradebookEnrollmentCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? EnrollmentCompletedSince { get; set; }
        public DateTimeOffset? EnrollmentCompletedBefore { get; set; }

        public DateTimeOffset? EnrollmentStartedSince { get; set; }
        public DateTimeOffset? EnrollmentStartedBefore { get; set; }
    }
}
