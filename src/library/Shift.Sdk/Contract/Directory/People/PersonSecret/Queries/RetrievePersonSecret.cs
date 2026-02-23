using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePersonSecret : Query<PersonSecretModel>
    {
        public Guid SecretId { get; set; }
    }
}