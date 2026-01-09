using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseFieldText : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public string Value { get; set; }

        public ModifyCourseFieldText(Guid courseId, CourseField courseField, string value)
        {
            AggregateIdentifier = courseId;
            CourseField = courseField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (StringHelper.EqualsCaseSensitive(course.Data.GetTextValue(CourseField), Value, true))
                return false;

            course.Apply(new CourseFieldTextModified(CourseField, Value));
            return true;
        }
    }
}
