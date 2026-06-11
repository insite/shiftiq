using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseFieldDateTimeOffset : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public DateTimeOffset? Value { get; set; }

        public ModifyCourseFieldDateTimeOffset(Guid courseId, CourseField courseField, DateTimeOffset? value)
        {
            AggregateIdentifier = courseId;
            CourseField = courseField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetDateOffsetValue(CourseField) == Value)
                return false;

            course.Apply(new CourseFieldDateTimeOffsetModified(CourseField, Value));
            return true;
        }
    }
}
