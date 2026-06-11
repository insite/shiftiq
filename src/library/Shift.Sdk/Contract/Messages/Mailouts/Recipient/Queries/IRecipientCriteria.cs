using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRecipientCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? MailoutId { get; set; }
        Guid? UserId { get; set; }

        string UserEmail { get; set; }

        DateTimeOffset? DeliveryCompletedSince { get; set; }
        DateTimeOffset? DeliveryCompletedBefore { get; set; }
        DateTimeOffset? DeliveryStartedSince { get; set; }
        DateTimeOffset? DeliveryStartedBefore { get; set; }
    }
}
