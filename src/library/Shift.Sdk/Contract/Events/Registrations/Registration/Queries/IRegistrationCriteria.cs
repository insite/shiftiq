using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRegistrationCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? AttendanceTakenSince { get; set; }
        DateTimeOffset? AttendanceTakenBefore { get; set; }
        DateTimeOffset? DistributionExpectedSince { get; set; }
        DateTimeOffset? DistributionExpectedBefore { get; set; }
        DateTimeOffset? EligibilityUpdatedSince { get; set; }
        DateTimeOffset? EligibilityUpdatedBefore { get; set; }
        DateTimeOffset? GradeAssignedSince { get; set; }
        DateTimeOffset? GradeAssignedBefore { get; set; }
        DateTimeOffset? GradePublishedSince { get; set; }
        DateTimeOffset? GradePublishedBefore { get; set; }
        DateTimeOffset? GradeReleasedSince { get; set; }
        DateTimeOffset? GradeReleasedBefore { get; set; }
        DateTimeOffset? GradeWithheldSince { get; set; }
        DateTimeOffset? GradeWithheldBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
        DateTimeOffset? RegistrationRequestedOnSince { get; set; }
        DateTimeOffset? RegistrationRequestedOnBefore { get; set; }
    }
}