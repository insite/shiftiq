using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Constant;

namespace InSite.Domain.Banks
{
    public class QuestionPublicationStatusChanged : Change
    {
        public Guid Question { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public PublicationStatus Status { get; set; }

        public QuestionPublicationStatusChanged(Guid question, PublicationStatus status)
        {
            Question = question;
            Status = status;
        }
    }
}
