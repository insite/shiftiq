using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityGradeItemConnected : Change
    {
        public Guid ActivityId { get; set; }
        public Guid? GradeItemId { get; set; }

        public CourseActivityGradeItemConnected(Guid activityId, Guid? gradeItemId)
        {
            ActivityId = activityId;
            GradeItemId = gradeItemId;
        }
    }
}
