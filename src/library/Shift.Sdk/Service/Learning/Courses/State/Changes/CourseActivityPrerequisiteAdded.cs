using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityPrerequisiteAdded : Change
    {
        public Guid ActivityId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public CourseActivityPrerequisiteAdded(Guid activityId, Prerequisite prerequisite)
        {
            ActivityId = activityId;
            Prerequisite = prerequisite;
        }
    }
}
