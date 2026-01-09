using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveRegistration : Query<RegistrationModel>
    {
        public Guid RegistrationIdentifier { get; set; }
    }
}