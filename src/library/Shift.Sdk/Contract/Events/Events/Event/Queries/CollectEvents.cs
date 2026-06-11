using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectEvents : Query<IEnumerable<EventModel>>, IEventCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? DistributionExpectedSince { get; set; }
        public DateTimeOffset? DistributionExpectedBefore { get; set; }
        public DateTimeOffset? DistributionOrderedSince { get; set; }
        public DateTimeOffset? DistributionOrderedBefore { get; set; }
        public DateTimeOffset? DistributionShippedSince { get; set; }
        public DateTimeOffset? DistributionShippedBefore { get; set; }
        public DateTimeOffset? DistributionTrackedSince { get; set; }
        public DateTimeOffset? DistributionTrackedBefore { get; set; }
        public DateTimeOffset? EventScheduledEndSince { get; set; }
        public DateTimeOffset? EventScheduledEndBefore { get; set; }
        public DateTimeOffset? EventScheduledStartSince { get; set; }
        public DateTimeOffset? EventScheduledStartBefore { get; set; }
        public DateTimeOffset? ExamMaterialReturnShipmentReceivedSince { get; set; }
        public DateTimeOffset? ExamMaterialReturnShipmentReceivedBefore { get; set; }
        public DateTimeOffset? ExamStartedSince { get; set; }
        public DateTimeOffset? ExamStartedBefore { get; set; }
        public DateTimeOffset? LastChangeTimeSince { get; set; }
        public DateTimeOffset? LastChangeTimeBefore { get; set; }
        public DateTimeOffset? RegistrationDeadlineSince { get; set; }
        public DateTimeOffset? RegistrationDeadlineBefore { get; set; }
        public DateTimeOffset? RegistrationLockedSince { get; set; }
        public DateTimeOffset? RegistrationLockedBefore { get; set; }
        public DateTimeOffset? RegistrationStartSince { get; set; }
        public DateTimeOffset? RegistrationStartBefore { get; set; }
    }
}