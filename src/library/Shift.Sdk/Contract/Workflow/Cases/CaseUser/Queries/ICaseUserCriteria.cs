using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseUserCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? CaseId { get; set; }
        Guid? UserId { get; set; }

        string CaseRole { get; set; }
    }
}