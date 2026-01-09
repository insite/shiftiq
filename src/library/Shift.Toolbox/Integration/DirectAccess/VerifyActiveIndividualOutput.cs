using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class VerifyActiveIndividualOutput
    {
        public bool Verified { get; set; }
        public string Reason { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public static VerifyActiveIndividualOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var fixedJson = json
                .Replace("True","true")
                .Replace("False","false");

            var response = JsonConvert.DeserializeObject<VerifyActiveIndividualOutput>(fixedJson);

            response.Raw = json;

            return response;
        }
    }
}