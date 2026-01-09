using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitRemoved : Change
    {
        public Guid UnitId { get; set; }

        public CourseUnitRemoved(Guid unitId)
        {
            UnitId = unitId;
        }
    }
}
