using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityFieldTextModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public string Value { get; set; }

        public CourseActivityFieldTextModified(Guid activityId, ActivityField activityField, string value)
        {
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }
    }
}
