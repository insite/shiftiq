using System;

using Shift.Common.Timeline.Commands;

using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    public class AddCourseEnrollment : Command, IHasRun
    {
        public Guid LearnerUserId { get; set; }
        public Guid CourseEnrollmentId { get; set; }
        public DateTimeOffset CourseStarted { get; set; }

        public AddCourseEnrollment(Guid courseId, Guid learnerUserId, Guid courseEnrollmentId, DateTimeOffset courseStarted)
        {
            AggregateIdentifier = courseId;
            LearnerUserId = learnerUserId;
            CourseEnrollmentId = courseEnrollmentId;
            CourseStarted = courseStarted;
        }

        bool IHasRun.Run(CourseAggregate course)
        {
            var enrollment = course.Data.Enrollments.Find(x => x.LearnerUserIdentifier == LearnerUserId);
            if (enrollment != null)
                return false;

            course.Apply(new CourseEnrollmentAdded(LearnerUserId, CourseEnrollmentId, CourseStarted));
            return true;
        }
    }
}
