using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class AddCourseModule : Command, IHasRun
    {
        public Guid UnitId { get; set; }
        public Guid ModuleId { get; set; }
        public int ModuleAsset { get; set; }
        public string ModuleName { get; set; }
        public ContentContainer ModuleContent { get; set; }

        public AddCourseModule(Guid courseId, Guid unitId, Guid moduleId, int moduleAsset, string moduleName, ContentContainer moduleContent)
        {
            AggregateIdentifier = courseId;
            UnitId = unitId;
            ModuleId = moduleId;
            ModuleAsset = moduleAsset;
            ModuleName = moduleName;
            ModuleContent = moduleContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var unit = course.Data.GetUnit(UnitId);
            if (unit == null)
                return false;

            course.Apply(new CourseModuleAdded(UnitId, ModuleId, ModuleAsset, ModuleName, ModuleContent));
            return true;
        }
    }
}
