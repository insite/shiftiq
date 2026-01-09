using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Common
{
    public class GoogleSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public GoogleTranslation Translation { get; set; }
    }

    public class GoogleTranslation
    {
        public List<string> Languages { get; set; }
        public GoogleServiceAccount Account { get; set; }
    }

    public class GoogleServiceAccount
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("project_id")]
        public string ProjectId { get; set; }

        [JsonProperty("private_key_id")]
        public string PrivateKeyId { get; set; }

        [JsonProperty("private_key")]
        public string PrivateKey { get; set; }

        [JsonProperty("client_email")]
        public string ClientEmail { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("auth_uri")]
        public string AuthUri { get; set; }

        [JsonProperty("token_uri")]
        public string TokenUri { get; set; }

        [JsonProperty("auth_provider_x509_cert_url")]
        public string AuthProviderX509CertUrl { get; set; }

        [JsonProperty("client_x509_cert_url")]
        public string ClientX509CertUrl { get; set; }
    }
}
