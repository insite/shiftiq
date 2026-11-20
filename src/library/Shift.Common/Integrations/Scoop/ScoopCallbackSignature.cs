using System;
using System.Security.Cryptography;
using System.Text;

namespace Shift.Common.Integration.Scoop
{
    /// <summary>
    /// Proof that a SCORM progress callback URL was minted by Shift iQ.
    /// </summary>
    /// <remarks>
    /// Scoop posts progress to whatever URL it was handed at launch, with no
    /// credential of its own (ProgressCallbackWorker posts a bare JSON body).
    /// Anything that authenticates the call therefore has to live in the URL,
    /// which is what this is: an HMAC over the activity and learner the URL
    /// names, keyed with the relay secret the two systems already share.
    ///
    /// What this stops: a forged completion from anyone who did not receive
    /// that exact launch URL. An activity id is visible in a portal URL and a
    /// learner id is not hard to come by, and until now those two were the
    /// whole of what a caller needed.
    ///
    /// What this does NOT stop: the learner named in the URL. For a Scoop
    /// launch the relay token travels through their browser, and the callback
    /// URL is base64 inside it, so they hold a validly signed URL for their own
    /// lesson. Scoop's own credential on the POST closes that: see
    /// ScoopCallbackHeaderSignature (TEC-1182), which the callback also requires.
    ///
    /// There is deliberately no expiry. A registration is long lived and the
    /// outbox retries, so a completion can legitimately arrive long after the
    /// launch that minted the URL. An expiry would turn those into abandoned
    /// callbacks, and a lost completion is a compliance record that quietly
    /// never arrives.
    /// </remarks>
    public static class ScoopCallbackSignature
    {
        /// <summary>The query string parameter carrying the signature.</summary>
        public const string ParameterName = "sig";

        /// <summary>
        /// The signature for one activity and learner pair.
        /// </summary>
        public static string Create(Guid activityId, Guid learnerId, string secret)
        {
            if (string.IsNullOrWhiteSpace(secret))
                throw new InvalidOperationException(
                    "Cannot sign a Scoop progress callback: Engine.Api.Scoop.Relay.Secret is not configured.");

            var payload = BuildPayload(activityId, learnerId);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));

                return Convert.ToBase64String(hash)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');
            }
        }

        /// <summary>
        /// Whether a presented signature matches the activity and learner in
        /// the callback route.
        /// </summary>
        public static bool IsValid(Guid activityId, Guid learnerId, string signature, string secret)
        {
            if (string.IsNullOrWhiteSpace(signature) || string.IsNullOrWhiteSpace(secret))
                return false;

            string expected;
            try
            {
                expected = Create(activityId, learnerId, secret);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return AreEqual(expected, signature.Trim());
        }

        /// <summary>
        /// The callback URL with its signature attached.
        /// </summary>
        public static string Sign(string callbackUrl, Guid activityId, Guid learnerId, string secret)
        {
            if (string.IsNullOrWhiteSpace(callbackUrl))
                return callbackUrl;

            var separator = callbackUrl.IndexOf('?') >= 0 ? "&" : "?";

            return callbackUrl + separator + ParameterName + "=" + Create(activityId, learnerId, secret);
        }

        // The two identifiers the route carries, in the order the route carries
        // them, so a signature cannot be replayed with the pair swapped.
        private static string BuildPayload(Guid activityId, Guid learnerId)
        {
            return activityId.ToString("D") + "/" + learnerId.ToString("D");
        }

        // Length-independent comparison, so a caller cannot learn the expected
        // signature one character at a time from how long the answer takes.
        private static bool AreEqual(string expected, string presented)
        {
            var left = Encoding.UTF8.GetBytes(expected);
            var right = Encoding.UTF8.GetBytes(presented);

            var difference = left.Length ^ right.Length;

            for (var i = 0; i < left.Length && i < right.Length; i++)
                difference |= left[i] ^ right[i];

            return difference == 0;
        }
    }
}
