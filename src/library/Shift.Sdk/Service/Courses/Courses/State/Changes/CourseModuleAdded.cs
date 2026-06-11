using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseModuleAdded : Change
    {
        public Guid UnitId { get; set; }
        public Guid ModuleId { get; set; }
        public int ModuleAsset { get; set; }
        public string ModuleName { get; set; }
        public ContentContainer ModuleContent { get; set; }

        public CourseModuleAdded(Guid unitId, Guid moduleId, int moduleAsset, string moduleName, ContentContainer moduleContent)
        {
            UnitId = unitId;
            ModuleId = moduleId;
            ModuleAsset = moduleAsset;
            ModuleName = moduleName;
            ModuleContent = moduleContent;
        }
    }
}
