using System;

namespace Shift.Common
{
    public class JwtRequest
    {
        public const int DefaultLifetime = 20 * 60; // 20 minutes = 1200 seconds

        public bool Debug { get; set; }

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
    }
}