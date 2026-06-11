using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectMailouts : Query<IEnumerable<MailoutModel>>, IMailoutCriteria
    {
        public Guid? OrganizationId { get; set; }

        public DateTimeOffset? MailoutCancelledSince { get; set; }
        public DateTimeOffset? MailoutCancelledBefore { get; set; }
        public DateTimeOffset? MailoutCompletedSince { get; set; }
        public DateTimeOffset? MailoutCompletedBefore { get; set; }
        public DateTimeOffset? MailoutScheduledSince { get; set; }
        public DateTimeOffset? MailoutScheduledBefore { get; set; }
        public DateTimeOffset? MailoutStartedSince { get; set; }
        public DateTimeOffset? MailoutStartedBefore { get; set; }
    }
}
