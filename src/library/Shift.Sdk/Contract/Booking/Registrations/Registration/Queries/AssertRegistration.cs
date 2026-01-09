using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertRegistration : Query<bool>
    {
        public Guid RegistrationIdentifier { get; set; }
    }
}