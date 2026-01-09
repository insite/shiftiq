using System;

namespace InSite.Application.Events.Read
{
    [Serializable]
    public class QEventTimer
    {
        public Guid EventIdentifier { get; set; }
        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }
        public Guid TriggerCommand { get; set; }
        public DateTimeOffset TriggerTime { get; set; }
    }
}
