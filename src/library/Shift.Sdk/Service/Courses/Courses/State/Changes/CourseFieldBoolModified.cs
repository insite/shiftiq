using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseFieldBoolModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public bool? Value { get; set; }

        public CourseFieldBoolModified(CourseField courseField, bool? value)
        {
            CourseField = courseField;
            Value = value;
        }
    }
}
