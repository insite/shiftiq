using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityLegacyPrerequisiteConnected : Change
    {
        public Guid ActivityId { get; set; }
        public Guid? PrerequisiteActivityId { get; set; }

        public CourseActivityLegacyPrerequisiteConnected(Guid activityId, Guid? prerequisiteActivityId)
        {
            ActivityId = activityId;
            PrerequisiteActivityId = prerequisiteActivityId;
        }
    }
}
