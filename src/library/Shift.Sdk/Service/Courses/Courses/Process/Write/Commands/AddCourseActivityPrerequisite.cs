using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class AddCourseActivityPrerequisite : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public AddCourseActivityPrerequisite(Guid courseId, Guid activityId, Prerequisite prerequisite)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            Prerequisite = prerequisite;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetActivity(ActivityId) == null)
                return false;

            course.Apply(new CourseActivityPrerequisiteAdded(ActivityId, Prerequisite));
            return true;
        }
    }
}
