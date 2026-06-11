using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class RenameCourseModule : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public string ModuleName { get; set; }

        public RenameCourseModule(Guid courseId, Guid moduleId, string moduleName)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            ModuleName = moduleName;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || StringHelper.EqualsCaseSensitive(module.ModuleName, ModuleName, true))
                return false;

            course.Apply(new CourseModuleRenamed(ModuleId, ModuleName));
            return true;
        }
    }
}
