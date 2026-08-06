using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICredentialCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementId { get; set; }
        Guid? UserId { get; set; }
        DateTimeOffset? ModifiedFrom { get; set; }
        DateTimeOffset? ModifiedBefore { get; set; }
        string PersonCode { get; set; }
    }
}
