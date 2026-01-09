using System;
using System.Globalization;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamIdForClassIdOutput
    {
        public bool Verified { get; set; }

        [JsonIgnore]
        public DateTime? ClassEndDate { get => DateTime.TryParseExact(ClassEndDateText, "dd-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) ? date : (DateTime?)null; }

        [JsonProperty(PropertyName = "classEndDate")]
        public string ClassEndDateText { get; set; }

        [JsonProperty(PropertyName = "exams")]
        public string[] Exams { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public static ExamIdForClassIdOutput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<ExamIdForClassIdOutput>(json);

            response.Raw = json;

            return response;
        }
    }
}