using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICredentialCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? AchievementId { get; set; }
        Guid? LearnerId { get; set; }
    }
}