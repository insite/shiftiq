using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseEnrollmentIncreased : Change
    {
        public Guid CourseEnrollmentId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseEnrollmentMessageType Message { get; set; }

        public CourseEnrollmentIncreased(Guid courseEnrollmentId, CourseEnrollmentMessageType message)
        {
            CourseEnrollmentId = courseEnrollmentId;
            Message = message;
        }
    }
}
