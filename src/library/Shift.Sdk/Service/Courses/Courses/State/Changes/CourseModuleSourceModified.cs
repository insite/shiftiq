using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleSourceModified : Change
    {
        public Guid ModuleId { get; set; }
        public Guid? Source { get; set; }

        public CourseModuleSourceModified(Guid moduleId, Guid? source)
        {
            ModuleId = moduleId;
            Source = source;
        }
    }
}
