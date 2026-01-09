using System;

using Shift.Common;

namespace InSite.Application.Registrations.Read
{
    [Serializable]
    public class QRegistrationTimerFilter : Filter
    {
        public Guid? EventIdentifier { get; set; }
        public Guid? RegistrationIdentifier { get; set; }
        public string TimerStatus { get; set; }
        public string TriggerDescription { get; set; }
        public DateTimeOffset? TriggerTimeSince { get; set; }
    }
}
