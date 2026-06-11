using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class DeleteCourse : Command, IHasRun
    {
        public DeleteCourse(Guid courseId)
        {
            AggregateIdentifier = courseId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            if (course.Data.IsRemoved)
                return false;

            course.Apply(new CourseDeleted());
            return true;
        }
    }
}
