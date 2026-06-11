using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseActivityContent : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public ContentContainer ActivityContent { get; set; }

        public ModifyCourseActivityContent(Guid courseId, Guid activityId, ContentContainer activityContent)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            ActivityContent = activityContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity == null || activity.Content.IsEqual(ActivityContent))
                return false;

            course.Apply(new CourseActivityContentModified(ActivityId, ActivityContent));
            return true;
        }
    }
}
