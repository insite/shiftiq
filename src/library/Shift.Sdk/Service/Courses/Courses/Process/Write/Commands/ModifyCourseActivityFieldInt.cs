using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityFieldInt : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public int? Value { get; set; }

        public ModifyCourseActivityFieldInt(Guid courseId,  Guid activityId, ActivityField activityField, int? value)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.GetIntValue(ActivityField) == Value)
                return false;

            course.Apply(new CourseActivityFieldIntModified(ActivityId, ActivityField, Value));
            return true;
        }
    }
}
