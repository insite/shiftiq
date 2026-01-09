using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCredential : Query<CredentialModel>
    {
        public Guid CredentialIdentifier { get; set; }
    }
}