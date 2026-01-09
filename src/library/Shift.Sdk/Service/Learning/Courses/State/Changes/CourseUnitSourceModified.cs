using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseUnitSourceModified : Change
    {
        public Guid UnitId { get; set; }
        public Guid? Source { get; set; }

        public CourseUnitSourceModified(Guid unitId, Guid? source)
        {
            UnitId = unitId;
            Source = source;
        }
    }
}
