using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitAdaptiveModified : Change
    {
        public Guid UnitId { get; set; }
        public bool IsAdaptive { get; set; }

        public CourseUnitAdaptiveModified(Guid unitId, bool isAdaptive)
        {
            UnitId = unitId;
            IsAdaptive = isAdaptive;
        }
    }
}
