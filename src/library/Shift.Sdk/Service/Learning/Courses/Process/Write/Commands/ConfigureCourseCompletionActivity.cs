using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConfigureCourseCompletionActivity : Command, IHasRun
    {
        public Guid? ActivityId { get; set; }

        public ConfigureCourseCompletionActivity(Guid courseId, Guid? activityId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetGuidValue(CourseField.CompletionActivityIdentifier) == ActivityId)
                return false;

            course.Apply(new CourseCompletionActivityConfigured(ActivityId));
            return true;
        }
    }
}
