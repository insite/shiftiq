using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public sealed class GeolocationData
    {
        [JsonProperty(PropertyName = "country")]
        public string Country { get; private set; }

        [JsonProperty(PropertyName = "region")]
        public string Region { get; private set; }

        [JsonProperty(PropertyName = "city")]
        public string City { get; private set; }

        [JsonConstructor]
        private GeolocationData()
        {

        }
    }
}