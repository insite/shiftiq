using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchRegistrations : Query<IEnumerable<RegistrationMatch>>, IRegistrationCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? AttendanceTakenSince { get; set; }
        public DateTimeOffset? AttendanceTakenBefore { get; set; }
        public DateTimeOffset? DistributionExpectedSince { get; set; }
        public DateTimeOffset? DistributionExpectedBefore { get; set; }
        public DateTimeOffset? EligibilityUpdatedSince { get; set; }
        public DateTimeOffset? EligibilityUpdatedBefore { get; set; }
        public DateTimeOffset? GradeAssignedSince { get; set; }
        public DateTimeOffset? GradeAssignedBefore { get; set; }
        public DateTimeOffset? GradePublishedSince { get; set; }
        public DateTimeOffset? GradePublishedBefore { get; set; }
        public DateTimeOffset? GradeReleasedSince { get; set; }
        public DateTimeOffset? GradeReleasedBefore { get; set; }
        public DateTimeOffset? GradeWithheldSince { get; set; }
        public DateTimeOffset? GradeWithheldBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
        public DateTimeOffset? RegistrationRequestedOnSince { get; set; }
        public DateTimeOffset? RegistrationRequestedOnBefore { get; set; }
    }
}