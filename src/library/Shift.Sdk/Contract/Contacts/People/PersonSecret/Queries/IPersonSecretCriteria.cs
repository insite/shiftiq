using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonSecretCriteria
    {
        QueryFilter Filter { get; set; }

        Guid? PersonId { get; set; }
        string SecretName { get; set; }

        DateTimeOffset? SecretExpirySince { get; set; }
        DateTimeOffset? SecretExpiryBefore { get; set; }
    }
}
