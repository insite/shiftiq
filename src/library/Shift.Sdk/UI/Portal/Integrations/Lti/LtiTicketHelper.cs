using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Shift.Sdk.UI
{
    public static class LtiTicketHelper
    {
        #region Fields

        private static readonly string[] UriRfc3986CharsToEscape = { "!", "*", "'", "(", ")" };

        #endregion

        #region Methods (LTI)

        public static LtiTicket GetTicket(string key, string url, LtiParameters parameters)
        {
            return GetTicket(key, new Uri(url), parameters);
        }

        private static LtiTicket GetTicket(string key, Uri url, LtiParameters parameters)
        {
            var p = parameters.GetParameters();
            foreach (var k in p.AllKeys.Where(k => string.IsNullOrWhiteSpace(p[k])))
                p.Remove(k);

            return GetTicketInternal(key, parameters.HttpMethod, url, p);
        }

        private static LtiTicket GetTicketInternal(string key, string httpMethod, Uri url, NameValueCollection parameters)
        {
            var signature = GenerateSignature(key, httpMethod, url, parameters);

            return new LtiTicket(url, signature, parameters);
        }

        #endregion

        #region Helpers (LTI)

        public static string GenerateSignature(string key, string httpMethod, Uri url, NameValueCollection parameters)
        {
            var temp = new NameValueCollection(parameters);

            var queryString = HttpUtility.ParseQueryString(url.Query);
            temp.Add(queryString);

            var signatureBase = GenerateSignatureBase(httpMethod, url, temp);
            var data = Encoding.ASCII.GetBytes(signatureBase);
            var hmacsha1Key = Encoding.ASCII.GetBytes(string.Format("{0}&", EscapeUriDataStringRfc3986(key)));

            byte[] hash;
            using (var hmacsha1 = new HMACSHA1 { Key = hmacsha1Key })
                hash = hmacsha1.ComputeHash(data);

            var signature = Convert.ToBase64String(hash);

            foreach (var queryKey in queryString.AllKeys)
                temp.Remove(queryKey);

            return signature;
        }

        private static string GenerateSignatureBase(string httpMethod, Uri url, NameValueCollection parameters)
        {
            var result = new StringBuilder();

            result.Append(EscapeUriDataStringRfc3986(httpMethod).ToUpperInvariant()).Append('&');

            var normalizedUrl = string.Format("{0}://{1}", url.Scheme.ToLowerInvariant(), url.Host.ToLowerInvariant());
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
                normalizedUrl += ":" + url.Port;
            normalizedUrl += url.AbsolutePath;

            result.Append(EscapeUriDataStringRfc3986(normalizedUrl)).Append('&');

            result.Append(
                EscapeUriDataStringRfc3986(
                    ToNormalizedString(parameters, new[] { "debug", "oauth_signature", "realm" })
                )
            );

            return result.ToString();
        }

        private static string EscapeUriDataStringRfc3986(string value)
        {
            var escaped = new StringBuilder(Uri.EscapeDataString(value));
            foreach (var s in UriRfc3986CharsToEscape)
                escaped.Replace(s, Uri.HexEscape(s[0]));

            return escaped.ToString();
        }

        private static string ToNormalizedString(this NameValueCollection collection, IList<string> excludedNames = null)
        {
            var list = new List<KeyValuePair<string, string>>();

            foreach (var key in collection.AllKeys)
            {
                if (excludedNames != null && excludedNames.Contains(key))
                    continue;

                var value = collection[key] ?? string.Empty;
                list.Add(new KeyValuePair<string, string>(
                    EscapeUriDataStringRfc3986(key),
                    EscapeUriDataStringRfc3986(value)
                ));
            }

            list.Sort((left, right) =>
                left.Key.Equals(right.Key, StringComparison.Ordinal)
                    ? string.Compare(left.Value, right.Value, StringComparison.Ordinal)
                    : string.Compare(left.Key, right.Key, StringComparison.Ordinal)
            );

            var normalizedString = new StringBuilder();
            foreach (var pair in list)
                normalizedString.Append('&').Append(pair.Key).Append('=').Append(pair.Value);

            return normalizedString.ToString().TrimStart('&');
        }

        #endregion
    }
}