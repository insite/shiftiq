using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseActivityPrerequisite : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public RemoveCourseActivityPrerequisite(Guid courseId, Guid activityId, Guid prerequisiteId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            PrerequisiteId = prerequisiteId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.Prerequisites.Find(x => x.Identifier == PrerequisiteId) == null)
                return false;

            course.Apply(new CourseActivityPrerequisiteRemoved(ActivityId, PrerequisiteId));
            return true;
        }
    }
}
