using Newtonsoft.Json;

namespace Shift.Common.MailgunWebhook
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