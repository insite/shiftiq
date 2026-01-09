using System;
using System.Security.Cryptography;

namespace InSite.Application.Contacts.Read
{
    public class QPersonSecret
    {
        public Guid PersonIdentifier { get; set; }
        public Guid SecretIdentifier { get; set; }

        public string SecretType { get; set; }
        public string SecretName { get; set; }
        public string SecretValue { get; set; }

        public DateTimeOffset SecretExpiry { get; set; }
        public int? SecretLifetimeLimit { get; set; }

        public virtual QPerson Person { get; set; }

        public static string CreateTokenSecret()
        {
            using (var provider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyBytes = new byte[64];
                provider.GetBytes(secretKeyBytes);
                return Convert.ToBase64String(secretKeyBytes);
            }
        }
    }
}
