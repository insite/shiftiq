using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseActivityLegacyPrerequisite : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid? PrerequisiteActivityId { get; set; }

        public ConnectCourseActivityLegacyPrerequisite(Guid courseId, Guid activityId, Guid? prerequisiteActivityId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            PrerequisiteActivityId = prerequisiteActivityId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity.GetGuidValue(ActivityField.PrerequisiteActivityIdentifier) == PrerequisiteActivityId)
                return false;

            course.Apply(new CourseActivityLegacyPrerequisiteConnected(ActivityId, PrerequisiteActivityId));
            return true;
        }
    }
}
