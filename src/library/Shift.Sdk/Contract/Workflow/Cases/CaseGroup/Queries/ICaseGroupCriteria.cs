using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseGroupCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? GroupIdentifier { get; set; }
        Guid? CaseIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }

        string CaseRole { get; set; }
    }
}