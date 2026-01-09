using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleRemoved : Change
    {
        public Guid ModuleId { get; set; }

        public CourseModuleRemoved(Guid moduleId)
        {
            ModuleId = moduleId;
        }
    }
}
