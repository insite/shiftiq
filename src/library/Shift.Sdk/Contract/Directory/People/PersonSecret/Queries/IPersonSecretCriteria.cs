using System;

using Shift.Common;

namespace Shift.Contract
{
    public interface IPersonSecretCriteria
    {
        QueryFilter Filter { get; set; }
        
        Guid? PersonIdentifier { get; set; }

        string SecretName { get; set; }
        string SecretType { get; set; }
        string SecretValue { get; set; }

        int? SecretLifetimeLimit { get; set; }

        DateTimeOffset? SecretExpiry { get; set; }
    }
}