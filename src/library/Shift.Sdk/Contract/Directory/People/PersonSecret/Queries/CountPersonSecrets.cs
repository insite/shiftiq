using System;

using Shift.Common;

namespace Shift.Contract
{
    public class CountPersonSecrets : Query<int>, IPersonSecretCriteria
    {
        public Guid? PersonIdentifier { get; set; }

        public string SecretName { get; set; }
        public string SecretType { get; set; }
        public string SecretValue { get; set; }

        public int? SecretLifetimeLimit { get; set; }

        public DateTimeOffset? SecretExpiry { get; set; }
    }
}