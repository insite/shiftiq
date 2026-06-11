using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseContentModified : Change
    {
        public ContentContainer CourseContent { get; set; }

        public CourseContentModified(ContentContainer courseContent)
        {
            CourseContent = courseContent;
        }
    }
}
