using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

using Shift.Common;

namespace InSite.Application.Courses.Write
{
    public class CreateCourse : Command, IHasRun, IHasAggregate
    {
        private CourseAggregate Course { get; set; }

        CourseAggregate IHasAggregate.Course => Course;

        public Guid OrganizationId { get; set; }
        public int CourseAsset { get; set; }
        public string CourseName { get; set; }
        public ContentContainer CourseContent { get; set; }

        public CreateCourse(Guid courseId, Guid organizationId, int courseAsset, string courseName, ContentContainer courseContent)
        {
            AggregateIdentifier = courseId;
            OrganizationId = organizationId;
            CourseAsset = courseAsset;
            CourseName = courseName;
            CourseContent = courseContent;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            Course = new CourseAggregate { AggregateIdentifier = AggregateIdentifier };
            Course.Apply(new CourseCreated(OrganizationId, CourseAsset, CourseName, CourseContent));
            return true;
        }
    }
}
