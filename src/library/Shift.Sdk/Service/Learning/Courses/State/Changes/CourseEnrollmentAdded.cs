using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseEnrollmentAdded : Change
    {
        public Guid LearnerUserId { get; set; }
        public Guid CourseEnrollmentId { get; set; }
        public DateTimeOffset CourseStarted { get; set; }

        public CourseEnrollmentAdded(Guid learnerUserId, Guid courseEnrollmentId, DateTimeOffset courseStarted)
        {
            LearnerUserId = learnerUserId;
            CourseEnrollmentId = courseEnrollmentId;
            CourseStarted = courseStarted;
        }
    }
}
