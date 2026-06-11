using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityMoved : Change
    {
        public Guid ActivityId { get; set; }
        public Guid MoveToModuleId { get; set; }

        public CourseActivityMoved(Guid activityId, Guid moveToModuleId)
        {
            ActivityId = activityId;
            MoveToModuleId = moveToModuleId;
        }
    }
}
