using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModulePrerequisiteAdded : Change
    {
        public Guid ModuleId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public CourseModulePrerequisiteAdded(Guid moduleId, Prerequisite prerequisite)
        {
            ModuleId = moduleId;
            Prerequisite = prerequisite;
        }
    }
}
