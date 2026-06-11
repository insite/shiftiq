using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IMailoutCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        DateTimeOffset? MailoutCancelledSince { get; set; }
        DateTimeOffset? MailoutCancelledBefore { get; set; }
        DateTimeOffset? MailoutCompletedSince { get; set; }
        DateTimeOffset? MailoutCompletedBefore { get; set; }
        DateTimeOffset? MailoutScheduledSince { get; set; }
        DateTimeOffset? MailoutScheduledBefore { get; set; }
        DateTimeOffset? MailoutStartedSince { get; set; }
        DateTimeOffset? MailoutStartedBefore { get; set; }
    }
}
