using System;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Courses
{
    public class CourseCreated : Change
    {
        public Guid OrganizationId { get; set; }
        public int CourseAsset { get; set; }
        public string CourseName { get; set; }
        public ContentContainer CourseContent { get; set; }

        public CourseCreated(Guid organizationId, int courseAsset, string courseName, ContentContainer courseContent)
        {
            OrganizationId = organizationId;
            CourseAsset = courseAsset;
            CourseName = courseName;
            CourseContent = courseContent;
        }
    }
}
