using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonCriteria
    {
        QueryFilter Filter { get; set; }

        string EmailExact { get; set; }
        string EventRole { get; set; }
        string FullName { get; set; }
        Guid? UserIdentifier { get; set; }
        Guid? OrganizationIdentifier { get; set; }
    }
}