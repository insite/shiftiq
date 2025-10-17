using System;

namespace InSite.Application.Registrations.Read
{
    public class QRegistrationTimer
    {
        public Guid RegistrationIdentifier { get; set; }
        public Guid TriggerCommand { get; set; }
        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }
        public DateTimeOffset TriggerTime { get; set; }
    }

    public class XRegistrationTimer
    {
        public Guid? EventIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid TriggerCommand { get; set; }
        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }
        public DateTimeOffset TriggerTime { get; set; }
    }
}
