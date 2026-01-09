using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface ICaseUserCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? CaseIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string CaseRole { get; set; }
    }
}