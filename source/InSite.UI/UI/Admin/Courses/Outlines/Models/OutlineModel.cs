using System;

using InSite.Application.Courses.Read;
using InSite.Domain.CourseObjects;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Admin.Courses.Outlines
{
    public class OutlineModel
    {
        public ContentContainer CourseContent { get; set; }
        public Course Course { get; set; }
        public TUnit Unit { get; set; }
        public TModule Module { get; set; }
        public TActivity Activity { get; set; }

        public OutlineModel(Course course, Guid activity)
        {
            Course = course;
            CourseContent = ServiceLocator.ContentSearch.GetBlock(course.Identifier);
            Activity = CourseSearch.SelectActivity(activity, x => x.Module.Unit.Course);

            if (Activity != null)
            {
                Module = CourseSearch.SelectModule(Activity.ModuleIdentifier);
             
                if (Module != null)
                    Unit = CourseSearch.SelectUnit(Module.UnitIdentifier);
            }
        }
    }
}