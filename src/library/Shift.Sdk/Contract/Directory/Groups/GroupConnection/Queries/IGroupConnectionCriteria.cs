using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupConnectionCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
    }
}