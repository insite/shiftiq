using System;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class IndividualRequestInput
    {
        [JsonProperty("individualId")]
        public string IndividualId { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("emailAddress")]
        public string Email { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime? DOB { get; set; }

        [JsonProperty("programName")]
        public string Program { get; set; }

        [JsonProperty("examId")]
        public string ExamFormCode { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public IndividualRequestInput Deserialize(string json) => JsonConvert.DeserializeObject<IndividualRequestInput>(json);

        public string Serialize() => JsonConvert.SerializeObject(this);

        public bool Equals(string json)
        {
            var o = Deserialize(json);

            return string.Equals(IndividualId, o.IndividualId)
                && string.Equals(FirstName, o.FirstName)
                && string.Equals(LastName, o.LastName)
                && DOB == o.DOB
                && string.Equals(Program, o.Program)
                && string.Equals(Email, o.Email)
                && string.Equals(ExamFormCode, o.ExamFormCode);
        }
    }
}