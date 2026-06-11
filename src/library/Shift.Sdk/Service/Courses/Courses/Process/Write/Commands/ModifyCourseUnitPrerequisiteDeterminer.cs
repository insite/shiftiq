using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitPrerequisiteDeterminer : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public string PrerequisiteDeterminer { get; set; }

        public ModifyCourseUnitPrerequisiteDeterminer(Guid courseId, Guid unitId, string prerequisiteDeterminer)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            PrerequisiteDeterminer = prerequisiteDeterminer;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || StringHelper.EqualsCaseSensitive(unit.PrerequisiteDeterminer, PrerequisiteDeterminer, true))
                return false;

            course.Apply(new CourseUnitPrerequisiteDeterminerModified(UnitId, PrerequisiteDeterminer));
            return true;
        }
    }
}
