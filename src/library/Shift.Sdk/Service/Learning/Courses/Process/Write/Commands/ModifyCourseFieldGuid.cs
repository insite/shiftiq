using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseFieldGuid : Command, IHasRun
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public CourseField CourseField { get; set; }

        public Guid? Value { get; set; }

        public ModifyCourseFieldGuid(Guid courseId, CourseField courseField, Guid? value)
        {
            AggregateIdentifier = courseId;
            CourseField = courseField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetGuidValue(CourseField) == Value)
                return false;

            course.Apply(new CourseFieldGuidModified(CourseField, Value));
            return true;
        }
    }
}
