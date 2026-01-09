using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertRegistrationInstructor : Query<bool>
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}