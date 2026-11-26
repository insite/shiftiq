using System;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integrations.DirectAccess
{
    public class ExamSubmissionResponse
    {
        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public bool IsSuccess => StatusCode == 200;

        public string ReceiptId { get; set; }

        public string SessionId { get; set; }

        public string IndividualId { get; set; }

        public string ExamId { get; set; }

        public string Code { get; set; }

        public string Raw { get; set; }

        public static ExamSubmissionResponse Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            try
            {
                var response = JsonConvert.DeserializeObject<ExamSubmissionResponse>(json);
                response.Raw = json;
                return response;
            }
            catch (Exception)
            {
                return new ExamSubmissionResponse { Raw = json };
            }
        }
    }
}