using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountRegistrationTimers : Query<int>, IRegistrationTimerCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }

        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }

        public DateTimeOffset? TriggerTime { get; set; }
    }
}