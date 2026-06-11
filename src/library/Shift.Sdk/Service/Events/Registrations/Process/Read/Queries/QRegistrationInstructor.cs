using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Registrations.Read
{
    public class QRegistrationInstructor
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public virtual QRegistration Registration { get; set; }
        public virtual VPerson Instructor { get; set; }
    }
}
