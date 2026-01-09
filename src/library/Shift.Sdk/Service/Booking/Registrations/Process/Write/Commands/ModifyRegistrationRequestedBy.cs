using System;

using Shift.Common.Timeline.Commands;

namespace InSite.Application.Registrations.Write
{
    public class ModifyRegistrationRequestedBy : Command
    {
        public Guid? RegistrationRequestedBy { get; set; }

        public ModifyRegistrationRequestedBy(Guid registration, Guid? registrationRequestedBy)
        {
            AggregateIdentifier = registration;
            RegistrationRequestedBy = registrationRequestedBy;
        }
    }
}
