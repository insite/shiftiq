using System;

namespace InSite.Domain.Courses
{
    [Serializable]
    public class Enrollment
    {
        public Guid Identifier { get; set; }

        public Guid LearnerUserIdentifier { get; set; }
        public DateTimeOffset CourseStarted { get; set; }
        public int MessageStalledSentCount { get; set; }
        public DateTimeOffset? CourseCompleted { get; set; }
        public int MessageCompletedSentCount { get; set; }
    }
}
