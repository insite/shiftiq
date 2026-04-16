using Newtonsoft.Json;

namespace InSite.Api.Models.MailgunWebhook
{
    public class AccountData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; private set; }

        [JsonConstructor]
        private AccountData()
        {

        }
    }
}