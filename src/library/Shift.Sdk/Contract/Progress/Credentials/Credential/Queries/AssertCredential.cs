using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCredential : Query<bool>
    {
        public Guid CredentialId { get; set; }
    }
}