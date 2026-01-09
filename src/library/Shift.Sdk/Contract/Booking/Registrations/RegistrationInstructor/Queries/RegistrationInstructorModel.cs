using System;

namespace Shift.Contract
{
    public partial class RegistrationInstructorModel
    {
        public Guid InstructorIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }
        public Guid RegistrationIdentifier { get; set; }
    }
}