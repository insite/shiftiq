using System;

namespace Shift.Common
{
    public class JwtRequest
    {
        public const int DefaultLifetime = 20 * 60; // 20 minutes = 1200 seconds

        public bool Debug { get; set; }

        public bool Paging { get; set; } = true;

        public string Secret { get; set; }

        public int? Lifetime { get; set; }

        public Guid? Organization { get; set; }

        public Guid? Agent { get; set; }

        public Guid? Subject { get; set; }
    }

    public class JwtResponse
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }

        public string Lifetime { get; set; }

        public string Paging { get; set; }
    }

    public class JwtIntrospectResponse
    {
        public object Jwt { get; set; }

        public object Principal { get; set; }
    }

    public class JwtValidateResult
    {
        public string Subject { get; set; }

        public string SignatureVerification { get; set; }

        public string ExpiryVerification { get; set; }

        public string AudienceVerification { get; set; }

        public string IssuerVerification { get; set; }
    }
}