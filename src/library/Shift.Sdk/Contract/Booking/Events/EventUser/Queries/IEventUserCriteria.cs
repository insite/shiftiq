using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IEventUserCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? EventId { get; set; }
        Guid? UserId { get; set; }

        string Role { get; set; }
    }
}