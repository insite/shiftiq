using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFormConditionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
    }
}