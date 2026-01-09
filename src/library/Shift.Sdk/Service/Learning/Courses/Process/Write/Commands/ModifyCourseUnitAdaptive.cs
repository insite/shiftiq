using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitAdaptive : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public bool IsAdaptive { get; set; }

        public ModifyCourseUnitAdaptive(Guid courseId, Guid unitId, bool isAdaptive)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            IsAdaptive = isAdaptive;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || unit.UnitIsAdaptive == IsAdaptive)
                return false;

            course.Apply(new CourseUnitAdaptiveModified(UnitId, IsAdaptive));
            return true;
        }
    }
}
