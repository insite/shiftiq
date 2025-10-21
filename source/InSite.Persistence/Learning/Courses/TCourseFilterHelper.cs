using System;
using System.Linq;

using InSite.Application.Courses.Read;

namespace InSite.Persistence
{
    public static class TCourseFilterHelper
    {
        public static IQueryable<TCourse> ApplyFilter(IQueryable<TCourse> query, TCourseFilter filter)
        {
            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogIdentifier == filter.CatalogIdentifier.Value);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrWhiteSpace(filter.CourseName))
                query = query.Where(x => x.CourseName.Contains(filter.CourseName));

            if (!string.IsNullOrWhiteSpace(filter.CourseLabel))
                query = query.Where(x => x.CourseLabel.Contains(filter.CourseLabel));

            if (filter.HasGradebook)
                query = query.Where(x => x.GradebookIdentifier != null && x.GradebookIdentifier != Guid.Empty);

            return query;
        }
    }
}
