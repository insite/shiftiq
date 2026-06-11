using System;

using Shift.Common;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventTimerFilter : Filter
    {
        public Guid? EventIdentifier { get; set; }
        public string TimerStatus { get; set; }
        public string TimerDescription { get; set; }
        public DateTimeOffset? TriggerTimeSince { get; set; }
    }
}
