using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IEventCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? DistributionExpectedSince { get; set; }
        DateTimeOffset? DistributionExpectedBefore { get; set; }
        DateTimeOffset? DistributionOrderedSince { get; set; }
        DateTimeOffset? DistributionOrderedBefore { get; set; }
        DateTimeOffset? DistributionShippedSince { get; set; }
        DateTimeOffset? DistributionShippedBefore { get; set; }
        DateTimeOffset? DistributionTrackedSince { get; set; }
        DateTimeOffset? DistributionTrackedBefore { get; set; }
        DateTimeOffset? EventScheduledEndSince { get; set; }
        DateTimeOffset? EventScheduledEndBefore { get; set; }
        DateTimeOffset? EventScheduledStartSince { get; set; }
        DateTimeOffset? EventScheduledStartBefore { get; set; }
        DateTimeOffset? ExamMaterialReturnShipmentReceivedSince { get; set; }
        DateTimeOffset? ExamMaterialReturnShipmentReceivedBefore { get; set; }
        DateTimeOffset? ExamStartedSince { get; set; }
        DateTimeOffset? ExamStartedBefore { get; set; }
        DateTimeOffset? LastChangeTimeSince { get; set; }
        DateTimeOffset? LastChangeTimeBefore { get; set; }
        DateTimeOffset? RegistrationDeadlineSince { get; set; }
        DateTimeOffset? RegistrationDeadlineBefore { get; set; }
        DateTimeOffset? RegistrationLockedSince { get; set; }
        DateTimeOffset? RegistrationLockedBefore { get; set; }
        DateTimeOffset? RegistrationStartSince { get; set; }
        DateTimeOffset? RegistrationStartBefore { get; set; }
    }
}