using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseActivitySurveyForm : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid? SurveyFormId { get; set; }

        public ConnectCourseActivitySurveyForm(Guid courseId, Guid activityId, Guid? surveyFormId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            SurveyFormId = surveyFormId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity.GetGuidValue(ActivityField.SurveyFormIdentifier) == SurveyFormId)
                return false;

            course.Apply(new CourseActivitySurveyFormConnected(ActivityId, SurveyFormId));
            return true;
        }
    }
}
