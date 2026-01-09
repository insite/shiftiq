using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseEnrollmentModified : Change
    {
        public Guid CourseEnrollmentId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseEnrollmentMessageType Message { get; set; }

        public int SentCount { get; set; }

        public CourseEnrollmentModified(Guid courseEnrollmentId, CourseEnrollmentMessageType message, int sentCount)
        {
            CourseEnrollmentId = courseEnrollmentId;
            Message = message;
            SentCount = sentCount;
        }
    }
}
