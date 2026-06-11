using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitPrerequisiteDeterminerModified : Change
    {
        public Guid UnitId { get; set; }
        public string PrerequisiteDeterminer { get; set; }

        public CourseUnitPrerequisiteDeterminerModified(Guid unitId, string prerequisiteDeterminer)
        {
            UnitId = unitId;
            PrerequisiteDeterminer = prerequisiteDeterminer;
        }
    }
}
