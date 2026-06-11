using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class RenameCourseUnit : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }

        public RenameCourseUnit(Guid courseId, Guid unitId, string unitName)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            UnitName = unitName;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || StringHelper.EqualsCaseSensitive(unit.UnitName, UnitName, true))
                return false;

            course.Apply(new CourseUnitRenamed(UnitId, UnitName));
            return true;
        }
    }
}
