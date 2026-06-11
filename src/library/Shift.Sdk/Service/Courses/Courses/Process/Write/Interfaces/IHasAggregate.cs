using InSite.Domain.Courses;

namespace InSite.Application.Courses.Write
{
    internal interface IHasAggregate
    {
        CourseAggregate Course { get; }
    }
}
