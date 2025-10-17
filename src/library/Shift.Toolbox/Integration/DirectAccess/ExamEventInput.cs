using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class ExamEventInput
    {
        [JsonProperty("eventId")]
        public int EventNumber { get; set; }

        [JsonProperty("eventStatus")]
        public string EventStatus { get; set; }

        [JsonProperty("deliveryMethod")]
        public string DeliveryMethod { get; set; }

        [JsonProperty("civicAddress")]
        public string[] CivicAddress { get; set; }

        [JsonProperty("tpContacts")]
        public string[] TPContacts { get; set; }

        [JsonProperty("organizationId")]
        public int? OrganizationId { get; set; }

        [JsonProperty("locationName")]
        public string Location { get; set; }

        [JsonProperty("roomNumber")]
        public string Room { get; set; }

        [JsonProperty("timeZone")]
        public string TZ { get; set; }

        [JsonProperty("registrationDate")]
        public string Registration { get; set; }

        [JsonProperty("startDate")]
        public string Start { get; set; }

        [JsonProperty("endDate")]
        public string End { get; set; }

        [JsonProperty("scheduledBy")]
        public int ScheduledBy { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonIgnore]
        public string Raw { get; set; }

        public string Serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static ExamEventInput Deserialize(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            var response = JsonConvert.DeserializeObject<ExamEventInput>(json);

            response.Raw = json;

            return response;
        }
    }
}