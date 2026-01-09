using System;

namespace Shift.Contract
{
    public class ModifyRegistrationTimer
    {
        public Guid? OrganizationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid TriggerCommand { get; set; }

        public string TimerDescription { get; set; }
        public string TimerStatus { get; set; }

        public DateTimeOffset TriggerTime { get; set; }
    }
}