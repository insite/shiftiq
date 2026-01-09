using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseActivityAssessmentForm : Command, IHasRun
    {
        public Guid ActivityId { get; set; }
        public Guid? AssessmentFormId { get; set; }

        public ConnectCourseActivityAssessmentForm(Guid courseId, Guid activityId, Guid? assessmentFormId)
        {
            AggregateIdentifier = courseId;
            ActivityId = activityId;
            AssessmentFormId = assessmentFormId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var activity = course.Data.GetActivity(ActivityId);
            if (activity.GetGuidValue(ActivityField.AssessmentFormIdentifier) == AssessmentFormId)
                return false;

            course.Apply(new CourseActivityAssessmentFormConnected(ActivityId, AssessmentFormId));
            return true;
        }
    }
}
