using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseModuleContentModified : Change
    {
        public Guid ModuleId { get; }
        public ContentContainer ModuleContent { get; }

        public CourseModuleContentModified(Guid moduleId, ContentContainer moduleContent)
        {
            ModuleId = moduleId;
            ModuleContent = moduleContent;
        }
    }
}
