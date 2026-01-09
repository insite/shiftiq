using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ConnectCourseFramework : Command, IHasRun
    {
        public Guid? FrameworkStandardId { get; set; }

        public ConnectCourseFramework(Guid courseId, Guid? frameworkStandardId)
        {
            AggregateIdentifier = courseId;
            FrameworkStandardId = frameworkStandardId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.GetGuidValue(CourseField.FrameworkStandardIdentifier) == FrameworkStandardId)
                return false;

            course.Apply(new CourseFrameworkConnected(FrameworkStandardId));
            return true;
        }
    }
}
