using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Registrations
{
    public class RegistrationRequestedByModified : Change
    {
        public Guid? RegistrationRequestedBy { get; set; }

        public RegistrationRequestedByModified(Guid? registrationRequestedBy)
        {
            RegistrationRequestedBy = registrationRequestedBy;
        }
    }
}
