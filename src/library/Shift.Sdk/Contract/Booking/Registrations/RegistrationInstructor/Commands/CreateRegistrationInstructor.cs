using System;

namespace Shift.Contract
{
    public class CreateRegistrationInstructor
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}