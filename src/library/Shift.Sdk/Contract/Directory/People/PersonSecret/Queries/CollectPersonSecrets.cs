using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectPersonSecrets : Query<IEnumerable<PersonSecretModel>>, IPersonSecretCriteria
    {
        public Guid? PersonIdentifier { get; set; }

        public string SecretName { get; set; }
        public string SecretType { get; set; }
        public string SecretValue { get; set; }

        public int? SecretLifetimeLimit { get; set; }

        public DateTimeOffset? SecretExpiry { get; set; }
    }
}