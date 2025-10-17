using System;

using InSite.Application.Contacts.Read;

namespace InSite.Application.Courses.Read
{
    public class TCourseUser
    {
        public Guid EnrollmentIdentifier { get; set; }

        public Guid CourseIdentifier { get; set; }
        public Guid UserIdentifier { get; set; }
        public DateTimeOffset CourseStarted { get; set; }
        public int MessageStalledSentCount { get; set; }

        public DateTimeOffset? CourseCompleted { get; set; }
        public int MessageCompletedSentCount { get; set; }

        public virtual TCourse Course { get; set; }
        public virtual VUser User { get; set; }
    }
}
