using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Shift.Common.Integration.Scoop
{
    /// <summary>
    /// Why a Scoop progress callback's signature header was, or was not, accepted.
    /// </summary>
    public enum ScoopCallbackHeaderCheck
    {
        Valid,
        Missing,
        Malformed,
        OutsideTolerance,
        Mismatch
    }

    /// <summary>
    /// Proof that a SCORM progress callback was sent by Scoop, not replayed by
    /// the learner.
    /// </summary>
    /// <remarks>
    /// TEC-1182. ScoopCallbackSignature proves the callback URL was minted by
    /// Shift iQ, but that URL reaches the learner's browser inside the relay
    /// token, so the learner holds a validly signed URL for their own lesson.
    /// This header is the part of the exchange they never see: Scoop sends
    /// X-OpenSCORM-Signature: t=&lt;unix seconds&gt;,v1=&lt;hex&gt;, where v1 is
    /// HMAC-SHA256 over "&lt;t&gt;.&lt;raw body&gt;" keyed with the UTF-8 bytes of the
    /// relay secret. Producing it needs the secret, which the learner never has.
    ///
    /// The two checks share a key and nothing else. The URL signature is
    /// base64url over the route identifiers with no timestamp; this is hex over
    /// the body with one. The body is verified as the bytes received, before
    /// anything parses it, because a re-serialized body would not match.
    ///
    /// Scoop signs at send time, so an outbox retry arrives with a fresh t and
    /// the tolerance only has to absorb clock skew between the two servers. It
    /// applies in both directions, so a t from the future is as stale as one
    /// from the past.
    /// </remarks>
    public static class ScoopCallbackHeaderSignature
    {
        /// <summary>The header carrying the signature.</summary>
        public const string HeaderName = "X-OpenSCORM-Signature";

        /// <summary>How far t may be from the server clock, either way.</summary>
        public static readonly TimeSpan Tolerance = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The header value Scoop sends for a body at a moment. Used by the
        /// tests; the controller only verifies.
        /// </summary>
        public static string Create(byte[] body, string secret, long unixSeconds)
        {
            var mac = ComputeMac(body, secret, unixSeconds);

            return "t=" + unixSeconds.ToString(CultureInfo.InvariantCulture) + ",v1=" + ToHex(mac);
        }

        /// <summary>
        /// Checks a presented header against the body exactly as received.
        /// </summary>
        public static ScoopCallbackHeaderCheck Verify(string header, byte[] body, string secret, DateTimeOffset now)
        {
            var hasHeader = !string.IsNullOrWhiteSpace(header);
            if (!hasHeader)
                return ScoopCallbackHeaderCheck.Missing;

            var hasSecret = !string.IsNullOrWhiteSpace(secret);
            if (!hasSecret || body == null)
                return ScoopCallbackHeaderCheck.Mismatch;

            var isParsed = TryParse(header, out var unixSeconds, out var presented);
            if (!isParsed)
                return ScoopCallbackHeaderCheck.Malformed;

            var distance = Math.Abs(now.ToUnixTimeSeconds() - unixSeconds);
            if (distance > Tolerance.TotalSeconds)
                return ScoopCallbackHeaderCheck.OutsideTolerance;

            var expected = ComputeMac(body, secret, unixSeconds);

            var isMatch = AreEqual(expected, presented);

            return isMatch ? ScoopCallbackHeaderCheck.Valid : ScoopCallbackHeaderCheck.Mismatch;
        }

        // HMAC-SHA256 over the ASCII timestamp, a dot, and the body bytes as
        // received, which is what Scoop computes over the UTF-8 bytes it sends.
        private static byte[] ComputeMac(byte[] body, string secret, long unixSeconds)
        {
            var prefix = Encoding.UTF8.GetBytes(unixSeconds.ToString(CultureInfo.InvariantCulture) + ".");

            var message = new byte[prefix.Length + body.Length];
            Buffer.BlockCopy(prefix, 0, message, 0, prefix.Length);
            Buffer.BlockCopy(body, 0, message, prefix.Length, body.Length);

            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                return hmac.ComputeHash(message);
        }

        // "t=<digits>,v1=<64 hex>", in either order, ignoring unknown parts so a
        // later v2 can be added alongside without breaking this reader.
        private static bool TryParse(string header, out long unixSeconds, out byte[] mac)
        {
            unixSeconds = 0;
            mac = null;

            var hasTime = false;

            foreach (var part in header.Split(','))
            {
                var separator = part.IndexOf('=');
                if (separator <= 0)
                    continue;

                var key = part.Substring(0, separator).Trim();
                var value = part.Substring(separator + 1).Trim();

                if (key == "t")
                    hasTime = long.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out unixSeconds);
                else if (key == "v1")
                    mac = FromHex(value);
            }

            return hasTime && mac != null;
        }

        private static string ToHex(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);

            foreach (var b in bytes)
                builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));

            return builder.ToString();
        }

        // Null unless the value is exactly one SHA-256 digest in hex.
        private static byte[] FromHex(string hex)
        {
            if (hex.Length != 64)
                return null;

            var bytes = new byte[32];

            for (var i = 0; i < bytes.Length; i++)
            {
                var isByte = byte.TryParse(hex.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out bytes[i]);
                if (!isByte)
                    return null;
            }

            return bytes;
        }

        // Constant time over equal-length digests, so the answer's timing says
        // nothing about how much of the signature was right.
        private static bool AreEqual(byte[] expected, byte[] presented)
        {
            var difference = expected.Length ^ presented.Length;

            for (var i = 0; i < expected.Length && i < presented.Length; i++)
                difference |= expected[i] ^ presented[i];

            return difference == 0;
        }
    }
}
