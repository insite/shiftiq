using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseFieldInt : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public int? Value { get; set; }

        public ModifyCourseFieldInt(Guid courseId, CourseField courseField, int? value)
        {
            AggregateIdentifier = courseId;
            CourseField = courseField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetIntValue(CourseField) == Value)
                return false;

            course.Apply(new CourseFieldIntModified(CourseField, Value));
            return true;
        }
    }
}
