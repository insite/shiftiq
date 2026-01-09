using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IRegistrationTimerCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? OrganizationIdentifier { get; set; }
        Guid? RegistrationIdentifier { get; set; }

        string TimerDescription { get; set; }
        string TimerStatus { get; set; }

        DateTimeOffset? TriggerTime { get; set; }
    }
}