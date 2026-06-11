using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityRemoved : Change
    {
        public Guid ActivityId { get; set; }

        public CourseActivityRemoved(Guid activityId)
        {
            ActivityId = activityId;
        }
    }
}
