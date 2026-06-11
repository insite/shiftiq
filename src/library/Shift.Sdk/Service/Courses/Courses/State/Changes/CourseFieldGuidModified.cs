using System;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseFieldGuidModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public Guid? Value { get; set; }

        public CourseFieldGuidModified(CourseField courseField, Guid? value)
        {
            CourseField = courseField;
            Value = value;
        }
    }
}
