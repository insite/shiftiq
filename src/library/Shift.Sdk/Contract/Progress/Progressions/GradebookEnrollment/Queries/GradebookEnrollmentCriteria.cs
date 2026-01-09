using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGradebookEnrollmentCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? GradebookIdentifier { get; set; }
        Guid? LearnerIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? PeriodIdentifier { get; set; }

        string EnrollmentComment { get; set; }

        int? EnrollmentRestart { get; set; }

        DateTimeOffset? EnrollmentCompleted { get; set; }
        DateTimeOffset? EnrollmentStarted { get; set; }
    }
}