using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IFileActivityCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? FileIdentifier { get; set; }
        Guid? UserIdentifier { get; set; }

        string ActivityChanges { get; set; }

        DateTimeOffset? ActivityTime { get; set; }
    }
}