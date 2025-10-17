using System;

namespace Shift.Common
{
    /// <summary>
    /// Implements a raw JSON Web Token (JWT).
    /// </summary>
    /// <see href="https://datatracker.ietf.org/doc/html/rfc7519" />
    /// <see href="https://jwt.io/" />
    public class EncodedJwt
    {
        public string Header { get; set; }
        public string Payload { get; set; }
        public string Signature { get; set; }

        public EncodedJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("JWT cannot be empty.");

            var parts = token.Split('.');

            if (parts.Length != 3)
                throw new ArgumentException("JWT must be a string with 3 parts delimited by a period.");

            Header = parts[0];
            Payload = parts[1];
            Signature = parts[2];
        }

        public EncodedJwt(string header, string payload, string signature)
        {
            Header = header;
            Payload = payload;
            Signature = signature;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Header) || string.IsNullOrEmpty(Payload) || string.IsNullOrEmpty(Signature))
                return null;

            return $"{Header}.{Payload}.{Signature}";
        }
    }
}