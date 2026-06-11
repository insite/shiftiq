using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class MoveCourseActivity : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid MoveToModuleId { get; set; }

        public MoveCourseActivity(Guid courseId, Guid activityId, Guid moveToModuleId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            MoveToModuleId = moveToModuleId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.Module.Identifier == MoveToModuleId)
                return false;

            course.Apply(new CourseActivityMoved(ActivityId, MoveToModuleId));
            return true;
        }
    }
}
