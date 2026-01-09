using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseModule : Command, IHasRun
    {
        public Guid ModuleId { get; set; }

        public RemoveCourseModule(Guid courseId, Guid moduleId)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null)
                return false;

            course.Apply(new CourseModuleRemoved(ModuleId));
            return true;
        }
    }
}
