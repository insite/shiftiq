using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IUserFieldCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string Name { get; set; }
        string ValueJson { get; set; }
        string ValueType { get; set; }
    }
}