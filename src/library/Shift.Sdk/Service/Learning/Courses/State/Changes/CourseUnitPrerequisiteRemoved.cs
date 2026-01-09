using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitPrerequisiteRemoved : Change
    {
        public Guid UnitId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public CourseUnitPrerequisiteRemoved(Guid unitId, Guid prerequisiteId)
        {
            UnitId = unitId;
            PrerequisiteId = prerequisiteId;
        }
    }
}
