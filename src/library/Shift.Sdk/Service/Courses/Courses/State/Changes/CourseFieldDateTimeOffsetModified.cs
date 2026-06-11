using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseFieldDateTimeOffsetModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public DateTimeOffset? Value { get; set; }

        public CourseFieldDateTimeOffsetModified(CourseField courseField, DateTimeOffset? value)
        {
            CourseField = courseField;
            Value = value;
        }
    }
}
