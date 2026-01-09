using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityFieldBool : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public bool? Value { get; set; }

        public ModifyCourseActivityFieldBool(Guid courseId,  Guid activityId, ActivityField activityField, bool? value)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.GetBoolValue(ActivityField) == Value)
                return false;

            course.Apply(new CourseActivityFieldBoolModified(ActivityId, ActivityField, Value));
            return true;
        }
    }
}
