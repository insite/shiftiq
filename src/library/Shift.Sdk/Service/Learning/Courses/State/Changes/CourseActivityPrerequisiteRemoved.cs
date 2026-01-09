using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityPrerequisiteRemoved : Change
    {
        public Guid ActivityId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public CourseActivityPrerequisiteRemoved(Guid activityId, Guid prerequisiteId)
        {
            ActivityId = activityId;
            PrerequisiteId = prerequisiteId;
        }
    }
}
