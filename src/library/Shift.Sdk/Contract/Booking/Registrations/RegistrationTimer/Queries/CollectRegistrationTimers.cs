using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectRegistrationTimers : Query<IEnumerable<RegistrationTimerModel>>, IRegistrationTimerCriteria
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }

        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }

        public DateTimeOffset? TriggerTime { get; set; }
    }
}