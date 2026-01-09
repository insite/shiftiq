using System;

using Newtonsoft.Json;

namespace InSite.Persistence.Integration.BCMail
{
    public class DistributionRequestWhen
    {
        public Guid ActivityIdentifier { get; set; }
        public int ActivityNumber { get; set; }
        public string ActivityBillingCode { get; set; }
        public string ActivityType { get; set; }
        public string ActivityDate { get; set; }
        public string ActivityTime { get; set; }
        public string DistributionExpected { get; set; }

        [JsonIgnore]
        public string ExamType { get; set; }
    }
}