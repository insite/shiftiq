using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseGroupCriteria : IQueryByOrganization
    {
        QueryFilter Filter { get; set; }

        Guid? GroupId { get; set; }
        Guid? CaseId { get; set; }

        string CaseRole { get; set; }
    }
}