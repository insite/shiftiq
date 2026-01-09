using System;

namespace Shift.Contract
{
    public class DeleteRegistrationInstructor
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}