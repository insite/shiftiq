using System;

namespace Shift.Contract
{
    public partial class PersonSecretModel
    {
        public Guid PersonIdentifier { get; set; }
        public Guid SecretIdentifier { get; set; }

        public string SecretName { get; set; }
        public string SecretType { get; set; }
        public string SecretValue { get; set; }

        public int? SecretLifetimeLimit { get; set; }

        public DateTimeOffset SecretExpiry { get; set; }
    }
}