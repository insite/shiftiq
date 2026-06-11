using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseFrameworkConnected : Change
    {
        public Guid? FrameworkStandardId { get; set; }

        public CourseFrameworkConnected(Guid? frameworkStandardId)
        {
            FrameworkStandardId = frameworkStandardId;
        }
    }
}
