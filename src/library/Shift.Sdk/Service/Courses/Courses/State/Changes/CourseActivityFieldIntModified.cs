using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityFieldIntModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public int? Value { get; set; }

        public CourseActivityFieldIntModified(Guid activityId, ActivityField activityField, int? value)
        {
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }
    }
}
