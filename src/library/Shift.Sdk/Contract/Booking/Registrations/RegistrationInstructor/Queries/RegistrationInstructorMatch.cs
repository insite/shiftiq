using System;

namespace Shift.Contract
{
    public partial class RegistrationInstructorMatch
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}