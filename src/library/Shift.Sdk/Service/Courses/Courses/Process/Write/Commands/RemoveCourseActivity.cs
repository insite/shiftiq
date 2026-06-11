using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseActivity : Command, IHasRun
    {
        public Guid ActivityId { get; set; }

        public RemoveCourseActivity(Guid courseId, Guid activityId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null)
                return false;

            course.Apply(new CourseActivityRemoved(ActivityId));
            return true;
        }
    }
}
