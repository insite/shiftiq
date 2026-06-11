using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitSequenceModified : Change
    {
        public Guid UnitId { get; set; }
        public int UnitSequence { get; set; }

        public CourseUnitSequenceModified(Guid unitId, int unitSequence)
        {
            UnitId = unitId;
            UnitSequence = unitSequence;
        }
    }
}
