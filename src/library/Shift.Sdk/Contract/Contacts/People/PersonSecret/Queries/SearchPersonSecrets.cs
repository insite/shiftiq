using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class SearchPersonSecrets : Query<IEnumerable<PersonSecretMatch>>, IPersonSecretCriteria
    {
        public Guid? PersonId { get; set; }
        public string SecretName { get; set; }

        public DateTimeOffset? SecretExpirySince { get; set; }
        public DateTimeOffset? SecretExpiryBefore { get; set; }
    }
}
