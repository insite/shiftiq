using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseModuleImageModified : Change
    {
        public Guid ModuleId { get; set; }
        public string Image { get; set; }

        public CourseModuleImageModified(Guid moduleId, string image)
        {
            ModuleId = moduleId;
            Image = image;
        }
    }
}
