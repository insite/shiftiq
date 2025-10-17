using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamEventOutput
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public string PublicationStatus
        {
            get
            {
                if (StatusCode == 200 || Regex.IsMatch(Message, @"^Event (Id )?(\d+).+(updated|created)"))
                    return "Succeeded";
                return "Failed";
            }
        }

        [JsonIgnore]
        public string PublicationErrors
        {
            get
            {
                if (PublicationStatus == "Failed")
                    return Message;
                return null;
            }
        }

        [JsonIgnore]
        public string Raw { get; set; }

        public static ExamEventOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<ExamEventOutput>(json);

            response.Raw = json;

            return response;
        }
    }
}