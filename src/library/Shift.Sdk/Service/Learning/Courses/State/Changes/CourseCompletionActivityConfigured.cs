using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseCompletionActivityConfigured : Change
    {
        public Guid? ActivityId { get; set; }

        public CourseCompletionActivityConfigured(Guid? activityId)
        {
            ActivityId = activityId;
        }
    }
}
