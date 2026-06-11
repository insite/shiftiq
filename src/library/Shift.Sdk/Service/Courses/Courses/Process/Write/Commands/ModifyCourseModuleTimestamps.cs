using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleTimestamps : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Modified { get; set; }

        public ModifyCourseModuleTimestamps(Guid courseId, Guid moduleId, Guid createdBy, DateTimeOffset created, Guid modifiedBy, DateTimeOffset modified)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            CreatedBy = createdBy;
            Created = created;
            ModifiedBy = modifiedBy;
            Modified = modified;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null)
                return false;

            course.Apply(new CourseModuleTimestampsModified(ModuleId, CreatedBy, Created, ModifiedBy, Modified));
            return true;
        }
    }
}
