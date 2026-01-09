using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitCodeModified : Change
    {
        public Guid UnitId { get; set; }
        public string UnitCode { get; set; }

        public CourseUnitCodeModified(Guid unitId, string unitCode)
        {
            UnitId = unitId;
            UnitCode = unitCode;
        }
    }
}
