using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace InSite.Web.Integration
{
    /// <summary>
    /// The relay token structure - must match Scoop's RelayToken class exactly.
    /// Do not modify the JSON property names or serialization logic.
    /// </summary>
    public class RelayToken
    {
        [JsonProperty("jti")]
        public string TokenId { get; set; }

        [JsonProperty("iat")]
        public long IssuedAt { get; set; }

        [JsonProperty("exp")]
        public long ExpiresAt { get; set; }

        [JsonProperty("iss")]
        public string Issuer { get; set; }

        [JsonProperty("sub")]
        public Guid UserId { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("org")]
        public string OrganizationCode { get; set; }

        [JsonProperty("org_id")]
        public Guid? OrganizationId { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("target")]
        public string TargetUrl { get; set; }

        [JsonProperty("exit")]
        public string ExitUrl { get; set; }

        [JsonIgnore]
        public string Signature { get; set; }

        public string ComputeSignature(string secret)
        {
            var payload = SerializeForSigning();
            return ComputeHmac(payload, secret);
        }

        public string Encode(string secret)
        {
            Signature = ComputeSignature(secret);
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            var bytes = Encoding.UTF8.GetBytes(json);
            var base64 = ToUrlSafeBase64(bytes);
            return base64 + "." + Signature;
        }

        private string SerializeForSigning()
        {
            // CRITICAL: Order and format must match Scoop exactly
            var signingData = new Dictionary<string, object>
            {
                ["jti"] = TokenId,
                ["iat"] = IssuedAt,
                ["exp"] = ExpiresAt,
                ["iss"] = Issuer,
                ["sub"] = UserId.ToString(),
                ["email"] = Email,
                ["name"] = Name,
                ["org"] = OrganizationCode,
                ["org_id"] = OrganizationId?.ToString(),
                ["roles"] = Roles,
                ["target"] = TargetUrl
            };

            return JsonConvert.SerializeObject(signingData, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.None
            });
        }

        private static string ComputeHmac(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return ToUrlSafeBase64(hashBytes);
            }
        }

        private static string ToUrlSafeBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    }
}
