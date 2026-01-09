using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Newtonsoft.Json;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityFieldDate : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public ActivityField ActivityField { get; set; }

        public DateTime? Value { get; set; }

        public ModifyCourseActivityFieldDate(Guid courseId,  Guid activityId, ActivityField activityField, DateTime? value)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityField = activityField;
            Value = value?.Date;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.GetDateValue(ActivityField) == Value)
                return false;

            course.Apply(new CourseActivityFieldDateModified(ActivityId, ActivityField, Value));
            return true;
        }
    }
}
