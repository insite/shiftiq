using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IGroupTagCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? GroupIdentifier { get; set; }

        string GroupTag { get; set; }
    }
}