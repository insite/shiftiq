using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseActivityContentModified : Change
    {
        public Guid ActivityId { get; set; }
        public ContentContainer ActivityContent { get; set; }

        public CourseActivityContentModified(Guid activityId, ContentContainer activityContent)
        {
            ActivityId = activityId;
            ActivityContent = activityContent;
        }
    }
}
