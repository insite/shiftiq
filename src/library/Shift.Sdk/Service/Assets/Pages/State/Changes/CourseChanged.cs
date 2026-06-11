using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Sites.Pages
{
    public class CourseChanged : Change
    {
        public Guid? Course { get; set; }
        public CourseChanged(Guid? course)
        {
            Course = course;
        }
    }
}
