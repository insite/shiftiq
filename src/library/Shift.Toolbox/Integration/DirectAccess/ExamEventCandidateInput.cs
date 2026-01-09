using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamEventCandidateInput
    {
        [JsonProperty("eventId")]
        public int EventNumber { get; set; }

        [JsonProperty("formStatus")]
        public string FormStatus { get; set; }

        [JsonProperty("resultStatus")]
        public string ResultStatus { get; set; }

        [JsonProperty("individualId")]
        public int IndividualNumber { get; set; }

        [JsonProperty("examId")]
        public string Exam { get; set; }

        [JsonProperty("awisExamName")]
        public string AWISExamName { get; set; }

        [JsonProperty("courseSessionId")]
        public string CourseSessionId { get; set; }

        [JsonProperty("tradeCode")]
        public int Trade { get; set; }

        [JsonProperty("calculator")]
        public string Calculator { get; set; }

        [JsonProperty("codeBook")]
        public string CodeBook { get; set; }

        [JsonProperty("active")]
        public string Active { get; set; }

        [JsonProperty("removed")]
        public string Removed { get; set; }

        [JsonProperty("removalReason")]
        public object RemovalReason { get; set; }

        [JsonProperty("dictionaryAllowed")]
        public string DictionaryAllowed { get; set; }

        [JsonProperty("eligible")]
        public string Eligible { get; set; }

        [JsonProperty("ineligibilityCodes")]
        public string[] IneligibleCodes { get; set; } = new string[0];

        [JsonProperty("ineligibilityCodesFormatted")]
        public string IneligibilityCodesFormatted { get; set; }

        [JsonProperty("ineligibilityOverride")]
        public string IneligibilityOverride { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("void")]
        public string Void { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ExamEventCandidateInput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<ExamEventCandidateInput>(json);
            response.Raw = json;

            return response;
        }
    }
}