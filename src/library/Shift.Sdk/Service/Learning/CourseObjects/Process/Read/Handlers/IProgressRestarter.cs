using System;

namespace InSite.Application.Courses.Read
{
    public interface IProgressRestarter
    {
        void Restart(Guid learner, Guid course, DateTimeOffset started);

        Guid GetProgressIdentifier(Guid learner, Guid gradebook, Guid gradeitem);
    }
}