using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    internal interface IHasRun
    {
        bool Run(CourseAggregate course);
    }
}
