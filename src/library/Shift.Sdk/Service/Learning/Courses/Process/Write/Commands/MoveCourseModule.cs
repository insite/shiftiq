using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class MoveCourseModule : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Guid MoveToUnitId { get; set; }

        public MoveCourseModule(Guid courseId, Guid moduleId, Guid moveToUnitId)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            MoveToUnitId = moveToUnitId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || module.Unit.Identifier == MoveToUnitId)
                return false;

            course.Apply(new CourseModuleMoved(ModuleId, MoveToUnitId));
            return true;
        }
    }
}
