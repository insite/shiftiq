using System.Collections.Generic;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class AdHocEventNotificationInput
    {
        [JsonProperty("eventId")]
        public int EventNumber { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("recipients")]
        public List<string> Recipients { get; set; }

        [JsonProperty("html")]
        public string HTML { get; set; }

        [JsonProperty("fileName")]
        public string FileName { get; set; }

        public AdHocEventNotificationInput()
        {
            Recipients = new List<string>();
        }

        [JsonIgnore]
        public string Raw { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static AdHocEventNotificationInput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<AdHocEventNotificationInput>(json);
            response.Raw = json;

            return response;
        }
    }
}