using System;
using System.Net.Http;

namespace Shift.Common.Integration
{
    // The credential for the Shift.Hub API contract: partition registration and the Google,
    // Premailer and ImageMagick integrations. Clients send it; Shift.Hub checks it on those routes.

    public static class HubApiKey
    {
        public const string HeaderName = "X-Hub-Api-Key";

        private static readonly string[] RegistrationPaths =
        {
            "/partitions",
            "/api/partitions",
        };

        private static readonly string[] IntegrationPrefixes =
        {
            "/google/",
            "/premailer/",
            "/imagemagick/",
        };

        // Liveness endpoints the hub serves under each integration prefix. Left open so an uptime
        // monitor needs no secret; they report a version and nothing else.
        private static readonly string[] OpenLeaves =
        {
            "status",
            "health",
            "version",
        };

        // The shared HttpClient is a process-wide singleton, so the key is set per-request rather
        // than on DefaultRequestHeaders to avoid leaking into unrelated callers.

        public static void Apply(HttpRequestMessage request, string apiKey)
        {
            if (!string.IsNullOrEmpty(apiKey))
                request.Headers.Add(HeaderName, apiKey);
        }

        // Whether a request path (with any PathBase already removed) belongs to the contract and
        // therefore requires the key.

        public static bool IsContractPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var trimmed = path.Length > 1 ? path.TrimEnd('/') : path;

            foreach (var registration in RegistrationPaths)
            {
                if (trimmed.Equals(registration, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            foreach (var prefix in IntegrationPrefixes)
            {
                if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var leaf = trimmed.Substring(Math.Min(prefix.Length, trimmed.Length));

                foreach (var open in OpenLeaves)
                {
                    if (leaf.Equals(open, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                return true;
            }

            return false;
        }

        // What a request presented on a hub route, for the hub to act on. Kept free of HTTP types so
        // the rules can be tested without hosting the hub.

        public enum Check
        {
            NotRequired,  // Not a contract route.
            Valid,        // The right key.
            Missing,      // No header at all: a client not yet redeployed.
            Invalid,      // A header carrying the wrong key.
            Unconfigured  // The hub itself has no key to compare against.
        }

        public static Check Evaluate(string path, string presented, string configured)
        {
            if (!IsContractPath(path))
                return Check.NotRequired;

            if (string.IsNullOrEmpty(configured))
                return Check.Unconfigured;

            if (string.IsNullOrEmpty(presented))
                return Check.Missing;

            return Matches(presented, configured) ? Check.Valid : Check.Invalid;
        }

        // Constant-time comparison, so response timing does not reveal how much of a guess was
        // right. Hand-written because netstandard2.0 has no CryptographicOperations.FixedTimeEquals.
        // False when either side is empty, so an unconfigured key admits nothing.

        public static bool Matches(string presented, string expected)
        {
            if (string.IsNullOrEmpty(presented) || string.IsNullOrEmpty(expected))
                return false;

            var difference = presented.Length ^ expected.Length;

            for (var i = 0; i < presented.Length; i++)
                difference |= presented[i] ^ expected[i % expected.Length];

            return difference == 0;
        }
    }
}
