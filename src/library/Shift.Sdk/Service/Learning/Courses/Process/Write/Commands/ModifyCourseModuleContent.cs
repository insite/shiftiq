using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleContent : Command, IHasRun
    {
        public Guid ModuleId { get; }
        public ContentContainer ModuleContent { get; }

        public ModifyCourseModuleContent(Guid courseId, Guid moduleId, ContentContainer moduleContent)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            ModuleContent = moduleContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.Content.IsEqual(ModuleContent))
                return false;

            course.Apply(new CourseModuleContentModified(ModuleId, ModuleContent));
            return true;
        }
    }
}
