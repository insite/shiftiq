using System;

namespace InSite.Application.Courses.Read
{
    public interface ICourseObjectStore
    {
        void InsertCourseUser(Guid course, Guid user);
        void IncreaseMessageStalledSentCount(Guid course, Guid enrollment);
        void IncreaseMessageCompletedSentCount(Guid course, Guid enrollment);
        void CompleteCourse(Guid course, Guid user, DateTimeOffset completed);
    }
}
