using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitPrerequisiteAdded : Change
    {
        public Guid UnitId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public CourseUnitPrerequisiteAdded(Guid unitId, Prerequisite prerequisite)
        {
            UnitId = unitId;
            Prerequisite = prerequisite;
        }
    }
}
