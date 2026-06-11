using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleCodeModified : Change
    {
        public Guid ModuleId { get; set; }
        public string ModuleCode { get; set; }

        public CourseModuleCodeModified(Guid moduleId, string moduleCode)
        {
            ModuleId = moduleId;
            ModuleCode = moduleCode;
        }
    }
}
