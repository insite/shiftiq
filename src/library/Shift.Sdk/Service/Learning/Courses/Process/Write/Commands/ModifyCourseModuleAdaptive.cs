using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleAdaptive : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public bool IsAdaptive { get; set; }

        public ModifyCourseModuleAdaptive(Guid courseId, Guid moduleId, bool isAdaptive)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            IsAdaptive = isAdaptive;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.ModuleIsAdaptive == IsAdaptive)
                return false;

            course.Apply(new CourseModuleAdaptiveModified(ModuleId, IsAdaptive));
            return true;
        }
    }
}
