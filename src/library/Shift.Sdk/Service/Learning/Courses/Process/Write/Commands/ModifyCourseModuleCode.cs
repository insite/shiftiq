using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleCode : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public string ModuleCode { get; set; }

        public ModifyCourseModuleCode(Guid courseId, Guid moduleId, string moduleCode)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            ModuleCode = moduleCode;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || StringHelper.EqualsCaseSensitive(module.ModuleCode, ModuleCode, true))
                return false;

            course.Apply(new CourseModuleCodeModified(ModuleId, ModuleCode));
            return true;
        }
    }
}
