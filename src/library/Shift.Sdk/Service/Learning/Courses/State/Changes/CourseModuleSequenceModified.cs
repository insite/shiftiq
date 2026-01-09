using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleSequenceModified : Change
    {
        public Guid ModuleId { get; set; }
        public int ModuleSequence { get; set; }

        public CourseModuleSequenceModified(Guid moduleId, int moduleSequence)
        {
            ModuleId = moduleId;
            ModuleSequence = moduleSequence;
        }
    }
}
