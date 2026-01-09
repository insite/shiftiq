using System;

namespace Shift.Contract
{
    public class ModifyRegistrationInstructor
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}