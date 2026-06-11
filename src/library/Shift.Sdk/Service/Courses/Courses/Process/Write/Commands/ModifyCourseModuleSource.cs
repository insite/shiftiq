using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleSource : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Guid? Source { get; set; }

        public ModifyCourseModuleSource(Guid courseId, Guid moduleId, Guid? source)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            Source = source;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.SourceIdentifier == Source)
                return false;

            course.Apply(new CourseModuleSourceModified(ModuleId, Source));
            return true;
        }
    }
}
