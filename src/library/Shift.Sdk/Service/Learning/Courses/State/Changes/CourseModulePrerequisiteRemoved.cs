using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModulePrerequisiteRemoved : Change
    {
        public Guid ModuleId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public CourseModulePrerequisiteRemoved(Guid moduleId, Guid prerequisiteId)
        {
            ModuleId = moduleId;
            PrerequisiteId = prerequisiteId;
        }
    }
}
