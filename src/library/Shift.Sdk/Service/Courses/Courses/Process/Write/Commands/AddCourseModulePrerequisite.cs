using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class AddCourseModulePrerequisite : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Prerequisite Prerequisite { get; set; }

        public AddCourseModulePrerequisite(Guid courseId, Guid moduleId, Prerequisite prerequisite)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            Prerequisite = prerequisite;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null)
                return false;

            course.Apply(new CourseModulePrerequisiteAdded(ModuleId, Prerequisite));
            return true;
        }
    }
}
