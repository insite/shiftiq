using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class Jwt : IJwt
    {
        #region Construction

        private Dictionary<ClaimName, List<string>> _claims;

        public Jwt()
        {
            _claims = new Dictionary<ClaimName, List<string>>();

            Initialize(null, null, null, null);
        }

        public Jwt(Dictionary<ClaimName, string> claims)
            : this(claims, null, null, null, null)
        {

        }

        public Jwt(Dictionary<ClaimName, string> claims,
            string subject, string issuer, string audience, DateTimeOffset? expiry)
        {
            _claims = new Dictionary<ClaimName, List<string>>();

            foreach (var claim in claims)
                SetValue(claim.Key, claim.Value, false);

            Initialize(subject, issuer, audience, expiry);
        }

        public Jwt(Dictionary<ClaimName, List<string>> claims)
        {
            _claims = claims;

            Initialize(null, null, null, null);
        }

        public Jwt(Dictionary<ClaimName, List<string>> claims,
            string subject, string issuer, string audience, DateTimeOffset? expiry)
        {
            _claims = new Dictionary<ClaimName, List<string>>(claims);

            Initialize(subject, issuer, audience, expiry);
        }

        private void Initialize(string subject, string issuer, string audience, DateTimeOffset? expiry)
        {
            // Many external systems (including Moodle) use Firebase to verify authentication
            // tokens. Firebase considers a token invalid when the value of the "iat" claim
            // represents a time in the future. If there is a slight clock skew between servers,
            // then this can cause a token to be rejected even though it is valid on the server that
            // generated it. To mitigate this risk, we set the "iat" and "nbf" claims to a time one
            // minute in the past.
            // Refer to https://firebase.google.com/docs/auth/admin/verify-id-tokens.

            var now = DateTimeOffset.UtcNow.AddMinutes(-1).ToUnixTimeSeconds();

            if (subject.IsNotEmpty())
                SetValue(ClaimName.Subject, subject);

            if (issuer.IsNotEmpty())
                SetValue(ClaimName.Issuer, issuer);

            if (audience.IsNotEmpty())
                SetValue(ClaimName.Audience, audience);

            // iat = The date and time this set of JWT claims was issued.
            SetValue(ClaimName.Issued, now.ToString());

            // nbf = The date and time before which this set of JWT claims must not be used.
            SetValue(ClaimName.NotBefore, now.ToString());

            // exp = The expiration date and time for this set of JWT claims.
            if (expiry.HasValue)
                SetValue(ClaimName.Expiry, expiry.Value.ToUnixTimeSeconds().ToString());

            Sanitize();
        }

        #endregion

        #region Interrogation

        public bool ContainsClaim(ClaimName name)
        {
            return _claims.ContainsKey(name);
        }

        public int CountClaims()
        {
            var n = 0;

            foreach (var key in _claims.Keys)
            {
                var values = _claims[key];

                if (values != null)
                    n += values.Count;
            }

            return n;
        }

        public string GetClaimValue(ClaimName name)
        {
            if (!_claims.ContainsKey(name))
                return null;

            var values = _claims[name];

            return values.First();
        }

        public List<string> GetClaimValues(ClaimName name)
        {
            if (!_claims.ContainsKey(name))
                return null;

            var values = _claims[name];

            return values;
        }

        /// <summary>
        /// Checks for the existence of a claim with a specific value.
        /// </summary>
        public bool HasExpectedClaimValue(ClaimName name, string expectedValue)
        {
            if (!_claims.ContainsKey(name))
                return false;

            var actualValues = _claims[name];

            if (actualValues == null || actualValues.Count == 0)
                return false;

            return expectedValue.MatchesAny(actualValues);
        }

        /// <summary>
        /// Determines if the token is now expired.
        /// </summary>
        public bool IsExpired()
        {
            var exp = Expiry;

            return exp != null && exp.Value < DateTimeOffset.UtcNow;
        }

        public double GetMinutesSinceExpiry()
        {
            if (!IsExpired())
                return 0;

            var exp = Expiry;

            if (exp == null)
                return 0;

            return (DateTimeOffset.UtcNow - exp.Value).TotalMinutes;
        }

        public double GetMinutesUntilExpiry()
        {
            if (IsExpired())
                return 0;

            var exp = Expiry;

            if (exp == null)
                return 0;

            return (exp.Value - DateTimeOffset.UtcNow).TotalMinutes;
        }

        /// <summary>
        /// Return the full set of claims as a dictionary of string-value lists.
        /// </summary>
        /// <remarks>
        /// Although most security claim types have only one value, it is important to remember that 
        /// some security claim types can have multiple values. For example, the value for a "role"
        /// is an array - and not a single item.
        /// </remarks>
        public Dictionary<ClaimName, List<string>> ToDictionary()
            => _claims;

        #endregion

        #region Properties

        public string Audience
        {
            get
            {
                var aud = GetClaimValue(ClaimName.Audience);

                if (aud.IsEmpty())
                    return null;

                return aud;
            }
            set
            {
                if (value == null)
                    Delete(ClaimName.Audience);
                else
                    SetValue(ClaimName.Audience, value.ToString());
            }
        }

        public DateTimeOffset? Expiry
        {
            get
            {
                var exp = GetClaimValue(ClaimName.Expiry);

                if (exp == null)
                    return null;

                return DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp));
            }
            set
            {
                if (value == null)
                    Delete(ClaimName.Expiry);
                else
                    SetValue(ClaimName.Expiry, value.Value.ToUnixTimeSeconds().ToString());
            }
        }

        public string Issuer
        {
            get
            {
                var iss = GetClaimValue(ClaimName.Issuer);

                if (iss.IsEmpty())
                    return null;

                return iss;
            }
            set
            {
                if (value == null)
                    Delete(ClaimName.Issuer);
                else
                    SetValue(ClaimName.Issuer, value.ToString());
            }
        }

        public int? Lifetime
        {
            get
            {
                var ttl = GetClaimValue(ClaimName.Lifetime);

                if (ttl == null)
                    return null;

                return int.Parse(ttl);
            }
            set
            {
                if (value == null)
                    Delete(ClaimName.Lifetime);
                else
                    SetValue(ClaimName.Lifetime, value.ToString());
            }
        }

        public string Subject
        {
            get
            {
                var sub = GetClaimValue(ClaimName.Subject);

                if (sub.IsEmpty())
                    return null;

                return sub;
            }
            set
            {
                if (value == null)
                    Delete(ClaimName.Subject);
                else
                    SetValue(ClaimName.Subject, value.ToString());
            }
        }

        public List<string> Roles
        {
            get
            {
                return GetClaimValues(ClaimName.Role);
            }
            set
            {
                if (value == null || value.Count() == 0)
                    Delete(ClaimName.Role);

                foreach (var item in value)
                    if (item.IsNotEmpty())
                        SetValue(ClaimName.Role, item, true);
            }
        }

        #endregion

        #region Helpers

        private void SetValue(ClaimName name, string value, bool allowMultipleValues = false)
        {
            if (value.IsEmpty())
            {
                Delete(name);
            }
            else
            {
                if (!_claims.ContainsKey(name))
                    _claims.Add(name, new List<string>());

                var values = _claims[name];

                if (values.Count == 0)
                    values.Add(value);
                else if (allowMultipleValues)
                    values.Add(value);
                else
                    values[0] = value;
            }
        }

        private void Delete(ClaimName name)
        {
            if (_claims.ContainsKey(name))
                _claims.Remove(name);
        }

        /// <summary>
        /// Sort claims alphabetically, ensure dictionary keys are lowercase, and remove empty values.
        /// </summary>
        private void Sanitize()
        {
            // Remove empty values for every key.

            foreach (var key in _claims.Keys)
            {
                var values = _claims[key];

                if (values != null)
                {
                    values.RemoveAll(item => item.IsEmpty());
                    if (values.Count == 0)
                        values = null;
                }

                // If there are no remaining values then remove the claim.

                if (values == null)
                    _claims.Remove(key);
            }

            // Sort the remaining items by key, ensuring keys are lowercase.

            _claims = _claims
                .OrderBy(kvp => kvp.Key) // Sort by key.
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.OrderBy(item => item).ToList() // Sort the values also.
                );
        }

        #endregion
    }
}