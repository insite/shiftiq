using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseFieldTextModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public string Value { get; set; }

        public CourseFieldTextModified(CourseField courseField, string value)
        {
            CourseField = courseField;
            Value = value;
        }
    }
}
