using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityFieldBoolModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public bool? Value { get; set; }

        public CourseActivityFieldBoolModified(Guid activityId, ActivityField activityField, bool? value)
        {
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }
    }
}
