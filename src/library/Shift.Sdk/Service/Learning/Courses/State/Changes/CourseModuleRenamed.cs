using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleRenamed : Change
    {
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }

        public CourseModuleRenamed(Guid moduleId, string moduleName)
        {
            ModuleId = moduleId;
            ModuleName = moduleName;
        }
    }
}
