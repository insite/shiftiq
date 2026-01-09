using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleMoved : Change
    {
        public Guid ModuleId { get; set; }
        public Guid MoveToUnitId { get; set; }

        public CourseModuleMoved(Guid moduleId, Guid moveToUnitId)
        {
            ModuleId = moduleId;
            MoveToUnitId = moveToUnitId;
        }
    }
}
