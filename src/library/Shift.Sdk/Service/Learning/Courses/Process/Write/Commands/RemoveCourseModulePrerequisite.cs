using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseModulePrerequisite : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Guid PrerequisiteId { get; set; }

        public RemoveCourseModulePrerequisite(Guid courseId, Guid moduleId, Guid prerequisiteId)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            PrerequisiteId = prerequisiteId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.Prerequisites.Find(x => x.Identifier == PrerequisiteId) == null)
                return false;

            course.Apply(new CourseModulePrerequisiteRemoved(ModuleId, PrerequisiteId));
            return true;
        }
    }
}
