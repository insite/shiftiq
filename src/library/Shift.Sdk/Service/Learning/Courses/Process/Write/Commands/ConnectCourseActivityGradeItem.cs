using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseActivityGradeItem : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid? GradeItemId { get; set; }

        public ConnectCourseActivityGradeItem(Guid courseId, Guid activityId, Guid? gradeItemId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            GradeItemId = gradeItemId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity.GetGuidValue(ActivityField.GradeItemIdentifier) == GradeItemId
                || GradeItemId.HasValue && course.Data.GetActivityByGradeItem(GradeItemId.Value) != null
                )
            {
                return false;
            }

            course.Apply(new CourseActivityGradeItemConnected(ActivityId, GradeItemId));
            return true;
        }
    }
}
