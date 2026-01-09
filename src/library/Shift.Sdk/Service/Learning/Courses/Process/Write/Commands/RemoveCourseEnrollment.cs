using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class RemoveCourseEnrollment : Command, IHasRun
    {
        public Guid CourseEnrollmentId { get; set; }

        public RemoveCourseEnrollment(Guid courseId, Guid courseEnrollmentId)
        {
            AggregateIdentifier = courseId;
            CourseEnrollmentId = courseEnrollmentId;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var enrollment = course.Data.Enrollments.Find(x => x.Identifier == CourseEnrollmentId);
            if (enrollment == null)
                return false;

            course.Apply(new CourseEnrollmentRemoved(CourseEnrollmentId));
            return true;
        }
    }
}
