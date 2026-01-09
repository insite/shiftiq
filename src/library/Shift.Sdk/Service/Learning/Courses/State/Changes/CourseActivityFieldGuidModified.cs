using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseActivityFieldGuidModified : Change
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public Guid? Value { get; set; }

        public CourseActivityFieldGuidModified(Guid activityId, ActivityField activityField, Guid? value)
        {
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }
    }
}
