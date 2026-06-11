using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPersonSecrets : Query<int>, IPersonSecretCriteria
    {
        public Guid? PersonId { get; set; }
        public string SecretName { get; set; }

        public DateTimeOffset? SecretExpirySince { get; set; }
        public DateTimeOffset? SecretExpiryBefore { get; set; }
    }
}
