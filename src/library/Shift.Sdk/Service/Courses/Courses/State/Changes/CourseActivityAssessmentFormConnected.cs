using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivityAssessmentFormConnected : Change
    {
        public Guid ActivityId { get; set; }
        public Guid? AssessmentFormId { get; set; }

        public CourseActivityAssessmentFormConnected(Guid activityId, Guid? assessmentFormId)
        {
            ActivityId = activityId;
            AssessmentFormId = assessmentFormId;
        }
    }
}
