using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveCredential : Query<CredentialModel>
    {
        public Guid CredentialId { get; set; }
    }
}