using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGradebookEnrollmentCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? OrganizationId { get; set; }

        DateTimeOffset? EnrollmentCompletedSince { get; set; }
        DateTimeOffset? EnrollmentCompletedBefore { get; set; }

        DateTimeOffset? EnrollmentStartedSince { get; set; }
        DateTimeOffset? EnrollmentStartedBefore { get; set; }
    }
}
