using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    public class CourseFieldIntModified : Change
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public int? Value { get; set; }

        public CourseFieldIntModified(CourseField courseField, int? value)
        {
            CourseField = courseField;
            Value = value;
        }
    }
}
