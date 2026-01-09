using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePersonSecret : Query<PersonSecretModel>
    {
        public Guid SecretIdentifier { get; set; }
    }
}