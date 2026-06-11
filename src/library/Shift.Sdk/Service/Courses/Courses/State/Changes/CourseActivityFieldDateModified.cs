using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityFieldDateModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public DateTime? Value { get; set; }

        public CourseActivityFieldDateModified(Guid activityId, ActivityField activityField, DateTime? value)
        {
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }
    }
}
