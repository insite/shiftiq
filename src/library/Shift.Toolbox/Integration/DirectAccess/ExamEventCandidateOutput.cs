using System.Text.RegularExpressions;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamEventCandidateOutput
    {
        public string Message { get; set; }

        [JsonIgnore]
        public string PublicationStatus
        {
            get
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    var success = @"^Individual (\d+) in Event (\d+) (updated|created)\.$";
                    if (Regex.IsMatch(Message, success))
                        return "Succeeded";
                }
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

        public static ExamEventCandidateOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<ExamEventCandidateOutput>(json);
            response.Raw = json;

            return response;
        }
    }
}