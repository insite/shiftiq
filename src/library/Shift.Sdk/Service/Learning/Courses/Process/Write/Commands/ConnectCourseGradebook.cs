using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseGradebook : Command, IHasRun
    {
        public Guid? GradebookId { get; set; }

        public ConnectCourseGradebook(Guid courseId, Guid? gradebookId)
        {
            AggregateIdentifier = courseId;
            GradebookId = gradebookId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetGuidValue(CourseField.GradebookIdentifier) == GradebookId)
                return false;

            course.Apply(new CourseGradebookConnected(GradebookId));
            return true;
        }
    }
}
