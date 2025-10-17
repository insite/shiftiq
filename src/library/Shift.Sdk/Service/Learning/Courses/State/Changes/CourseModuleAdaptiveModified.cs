using System;

using Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleAdaptiveModified : Change
    {
        public Guid ModuleId { get; set; }
        public bool IsAdaptive { get; set; }

        public CourseModuleAdaptiveModified(Guid moduleId, bool isAdaptive)
        {
            ModuleId = moduleId;
            IsAdaptive = isAdaptive;
        }
    }
}
