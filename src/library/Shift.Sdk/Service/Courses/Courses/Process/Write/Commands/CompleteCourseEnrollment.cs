using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class CompleteCourseEnrollment : Command, IHasRun
    {
        public Guid CourseEnrollmentId { get; set; }
        public DateTimeOffset? Completed { get; set; }

        public CompleteCourseEnrollment(Guid courseId, Guid courseEnrollmentId, DateTimeOffset? completed)
        {
            AggregateIdentifier = courseId;
            CourseEnrollmentId = courseEnrollmentId;
            Completed = completed;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var enrollment = course.Data.Enrollments.Find(x => x.Identifier == CourseEnrollmentId);
            if (enrollment == null || enrollment.CourseCompleted == Completed)
                return false;

            course.Apply(new CourseEnrollmentCompleted(CourseEnrollmentId, Completed));
            return true;
        }
    }
}
