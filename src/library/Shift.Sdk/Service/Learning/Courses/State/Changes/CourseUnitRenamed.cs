using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitRenamed : Change
    {
        public Guid UnitId { get; set; }
        public string UnitName { get; set; }

        public CourseUnitRenamed(Guid unitId, string unitName)
        {
            UnitId = unitId;
            UnitName = unitName;
        }
    }
}
