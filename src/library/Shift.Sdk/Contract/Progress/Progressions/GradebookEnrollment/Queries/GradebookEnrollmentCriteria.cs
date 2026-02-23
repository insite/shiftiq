using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGradebookEnrollmentCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? GradebookId { get; set; }
        Guid? LearnerId { get; set; }
        Guid? OrganizationId { get; set; }
        Guid? PeriodId { get; set; }

        string EnrollmentComment { get; set; }

        int? EnrollmentRestart { get; set; }

        DateTimeOffset? EnrollmentCompleted { get; set; }
        DateTimeOffset? EnrollmentStarted { get; set; }
    }
}