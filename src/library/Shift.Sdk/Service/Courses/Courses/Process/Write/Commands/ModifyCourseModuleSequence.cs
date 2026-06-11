using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleSequence : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public int ModuleSequence { get; set; }

        public ModifyCourseModuleSequence(Guid courseId, Guid moduleId, int moduleSequence)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            ModuleSequence = moduleSequence;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.ModuleSequence == ModuleSequence)
                return false;

            course.Apply(new CourseModuleSequenceModified(ModuleId, ModuleSequence));
            return true;
        }
    }
}
