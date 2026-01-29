using System.Text.RegularExpressions;

namespace Shift.Common
{
    public class TokenRelaySettings
    {
        public const int DefaultLifetimeInMinutes = 10;

        /// <summary>
        /// Enable token relay authentication (vs shared cookie).
        /// When enabled, Scoop expects authentication via signed tokens from Shift.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Shared secret for HMAC signing of relay tokens.
        /// Must match the secret configured in all Shift deployments.
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// How long relay tokens are valid.
        /// Should be long enough for redirects but short for security.
        /// </summary>
        public string Lifetime { get; set; }

        public int GetLifetimeInMinutes()
        {
            var match = Regex.Match(Lifetime, @"(\d+) minutes?");

            if (!match.Success)
                return DefaultLifetimeInMinutes;

            var minutes = int.Parse(match.Groups[1].Value);

            return minutes;
        }
    }
}