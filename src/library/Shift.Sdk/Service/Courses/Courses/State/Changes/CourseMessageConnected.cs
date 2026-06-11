using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseMessageConnected : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseMessageType MessageType { get; set; }

        public Guid? MessageId { get; set; }
        public int? AfterDays { get; set; }
        public int? MaxCount { get; set; }

        public CourseMessageConnected(CourseMessageType messageType, Guid? messageId, int? afterDays, int? maxCount)
        {
            MessageType = messageType;
            MessageId = messageId;
            AfterDays = afterDays;
            MaxCount = maxCount;
        }
    }
}
