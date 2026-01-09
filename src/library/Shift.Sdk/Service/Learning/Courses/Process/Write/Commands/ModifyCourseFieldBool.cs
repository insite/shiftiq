using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseFieldBool : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public bool? Value { get; set; }

        public ModifyCourseFieldBool(Guid courseId, CourseField courseField, bool? value)
        {
            AggregateIdentifier = courseId;
            CourseField = courseField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetBoolValue(CourseField) == Value)
                return false;

            course.Apply(new CourseFieldBoolModified(CourseField, Value));
            return true;
        }
    }
}
