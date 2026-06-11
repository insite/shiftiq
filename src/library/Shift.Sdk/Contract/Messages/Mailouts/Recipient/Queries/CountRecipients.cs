using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountRecipients : Query<int>, IRecipientCriteria
    {
        public Guid? MailoutId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }

        public string UserEmail { get; set; }

        public DateTimeOffset? DeliveryCompletedSince { get; set; }
        public DateTimeOffset? DeliveryCompletedBefore { get; set; }
        public DateTimeOffset? DeliveryStartedSince { get; set; }
        public DateTimeOffset? DeliveryStartedBefore { get; set; }
    }
}
