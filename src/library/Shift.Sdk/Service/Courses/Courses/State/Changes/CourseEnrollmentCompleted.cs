using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseEnrollmentCompleted : Change
    {
        public Guid CourseEnrollmentId { get; set; }
        public DateTimeOffset? Completed { get; set; }

        public CourseEnrollmentCompleted(Guid courseEnrollmentId, DateTimeOffset? completed)
        {
            CourseEnrollmentId = courseEnrollmentId;
            Completed = completed;
        }
    }
}
