using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertCredential : Query<bool>
    {
        public Guid CredentialIdentifier { get; set; }
    }
}