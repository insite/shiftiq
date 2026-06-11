using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModulePrerequisiteDeterminerModified : Change
    {
        public Guid ModuleId { get; set; }
        public string PrerequisiteDeterminer { get; set; }

        public CourseModulePrerequisiteDeterminerModified(Guid moduleId, string prerequisiteDeterminer)
        {
            ModuleId = moduleId;
            PrerequisiteDeterminer = prerequisiteDeterminer;
        }
    }
}
