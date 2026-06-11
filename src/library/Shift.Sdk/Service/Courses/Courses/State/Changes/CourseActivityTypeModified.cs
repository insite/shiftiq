using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityTypeModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityType Type { get; set; }

        public CourseActivityTypeModified(Guid activityId, ActivityType type)
        {
            ActivityId = activityId;
            Type = type;
        }
    }
}
