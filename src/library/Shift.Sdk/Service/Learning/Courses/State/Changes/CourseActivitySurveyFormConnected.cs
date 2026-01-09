using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseActivitySurveyFormConnected : Change
    {
        public Guid ActivityId { get; set; }
        public Guid? SurveyFormId { get; set; }

        public CourseActivitySurveyFormConnected(Guid activityId, Guid? surveyFormId)
        {
            ActivityId = activityId;
            SurveyFormId = surveyFormId;
        }
    }
}
