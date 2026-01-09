using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityType : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityType Type { get; set; }

        public ModifyCourseActivityType(Guid courseId, Guid activityId, ActivityType type)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            Type = type;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.ActivityType == Type)
                return false;

            course.Apply(new CourseActivityTypeModified(ActivityId, Type));
            return true;
        }
    }
}
