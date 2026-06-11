using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseGradebookConnected : Change
    {
        public Guid? GradebookId { get; set; }

        public CourseGradebookConnected(Guid? gradebookId)
        {
            GradebookId = gradebookId;
        }
    }
}
