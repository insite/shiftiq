using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModulePrerequisiteDeterminer : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public string PrerequisiteDeterminer { get; set; }

        public ModifyCourseModulePrerequisiteDeterminer(Guid courseId, Guid moduleId, string prerequisiteDeterminer)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            PrerequisiteDeterminer = prerequisiteDeterminer;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || StringHelper.EqualsCaseSensitive(module.PrerequisiteDeterminer, PrerequisiteDeterminer, true))
                return false;

            course.Apply(new CourseModulePrerequisiteDeterminerModified(ModuleId, PrerequisiteDeterminer));
            return true;
        }
    }
}
