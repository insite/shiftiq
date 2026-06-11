using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class ModifyCourseModuleImage : Command, IHasRun
    {
        public Guid ModuleId { get; set; }
        public string Image { get; set; }

        public ModifyCourseModuleImage(Guid courseId, Guid moduleId, string image)
        {
            AggregateIdentifier = courseId;
            ModuleId = moduleId;
            Image = image;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var module = course.Data.GetModule(ModuleId);
            if (module == null || StringHelper.EqualsCaseSensitive(module.ModuleImage, Image, true))
                return false;

            course.Apply(new CourseModuleImageModified(ModuleId, Image));
            return true;
        }
    }
}
