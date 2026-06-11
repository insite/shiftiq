using System;

using Shift.Common.Timeline.Changes;

namespace InSite.Domain.Courses
{
    public class CourseEnrollmentRemoved : Change
    {
        public Guid CourseEnrollmentId { get; set; }

        public CourseEnrollmentRemoved(Guid courseEnrollmentId)
        {
            CourseEnrollmentId = courseEnrollmentId;
        }
    }
}
