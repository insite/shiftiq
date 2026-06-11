using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseUnitCode : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public string UnitCode { get; set; }

        public ModifyCourseUnitCode(Guid courseId, Guid unitId, string unitCode)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            UnitCode = unitCode;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null || StringHelper.EqualsCaseSensitive(unit.UnitCode, UnitCode, true))
                return false;

            course.Apply(new CourseUnitCodeModified(UnitId, UnitCode));
            return true;
        }
    }
}
