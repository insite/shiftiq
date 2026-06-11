using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityFieldGuid : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public Guid? Value { get; set; }

        public ModifyCourseActivityFieldGuid(Guid courseId,  Guid activityId, ActivityField activityField, Guid? value)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.GetGuidValue(ActivityField) == Value)
                return false;

            course.Apply(new CourseActivityFieldGuidModified(ActivityId, ActivityField, Value));
            return true;
        }
    }
}
