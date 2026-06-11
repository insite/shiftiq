using System;
using System.Collections.Generic;

using InSite.Application.Contacts.Read;
using InSite.Application.Invoices.Read;

namespace InSite.Application.Courses.Read
{
    public class QCourseEnrollment
    {
        public Guid CourseEnrollmentIdentifier { get; set; }
        public Guid CourseIdentifier { get; set; }
        public Guid LearnerUserIdentifier { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public int MessageCompletedSentCount { get; set; }
        public int MessageStalledSentCount { get; set; }

        public DateTimeOffset? CourseCompleted { get; set; }
        public DateTimeOffset CourseStarted { get; set; }

        public virtual QCourse Course { get; set; }
        public virtual QUser LearnerUser { get; set; }

        public virtual ICollection<TCourseDistribution> CourseDistributions { get; set; } = new HashSet<TCourseDistribution>();
    }
}
