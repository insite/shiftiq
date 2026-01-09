using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class AdHocEventNotificationOutput
    {
        public string Message { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public static AdHocEventNotificationOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<AdHocEventNotificationOutput>(json);
            response.Raw = json;

            return response;
        }
    }
}