using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveRegistrationInstructor : Query<RegistrationInstructorModel>
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}