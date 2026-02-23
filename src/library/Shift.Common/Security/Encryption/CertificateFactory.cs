using System;
using System.Security.Cryptography.X509Certificates;

namespace Shift.Common
{
    public static class CertificateFactory
    {
        public static X509Certificate2 Create(string encoded, string password)
        {
            if (string.IsNullOrEmpty(encoded))
                throw new ArgumentException("Certificate data cannot be null or empty.", nameof(encoded));

            byte[] decoded;
            try
            {
                decoded = Convert.FromBase64String(encoded);
            }
            catch (FormatException ex)
            {
                throw new ArgumentException("Certificate data is not valid Base64.", nameof(encoded), ex);
            }

            try
            {
                var certificate = new X509Certificate2(decoded, password, X509KeyStorageFlags.UserKeySet);
                return certificate;
            }
            catch (System.Security.Cryptography.CryptographicException ex)
            {
                throw new ArgumentException("Failed to create certificate. The data may be corrupted or the password may be incorrect.", nameof(encoded), ex);
            }
        }
    }
}
