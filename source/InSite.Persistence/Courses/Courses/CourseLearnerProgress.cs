using System;

namespace InSite.Persistence
{
    public class CourseLearnerProgress
    {
        public Guid LearnerIdentifier { get; set; }
        public DateTimeOffset? CourseCompleted { get; set; }
        public int CompletedCount { get; set; }
    }
}
