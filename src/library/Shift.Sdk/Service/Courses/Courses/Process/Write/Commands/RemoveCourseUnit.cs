using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseUnit : Command, IHasRun
    {
        public Guid UnitId { get; set; }

        public RemoveCourseUnit(Guid courseId, Guid unitId)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null)
                return false;

            course.Apply(new CourseUnitRemoved(UnitId));
            return true;
        }
    }
}
