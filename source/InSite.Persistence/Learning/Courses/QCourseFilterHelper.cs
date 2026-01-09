using System;
using System.Linq;

using InSite.Application.Courses.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public static class QCourseFilterHelper
    {
        internal static IQueryable<QCourse> ApplyFilter(IQueryable<QCourse> query, QCourseFilter filter, InternalDbContext context)
        {
            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogIdentifier == filter.CatalogIdentifier.Value);

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier.Value);
            else if (filter.HasGradebook == true)
                query = query.Where(x => x.GradebookIdentifier != null && x.GradebookIdentifier != Guid.Empty);
            else if (filter.HasGradebook == false)
                query = query.Where(x => x.GradebookIdentifier == null || x.GradebookIdentifier == Guid.Empty);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrWhiteSpace(filter.CourseName))
                query = query.Where(x => x.CourseName.Contains(filter.CourseName));

            if (!string.IsNullOrWhiteSpace(filter.CourseLabel))
                query = query.Where(x => x.CourseLabel.Contains(filter.CourseLabel));

            if (filter.GradebookTitle.IsNotEmpty())
                query = query.Where(x => x.Gradebook.GradebookTitle.Contains(filter.GradebookTitle));

            if (filter.HasWebPage.HasValue)
            {
                var subQuery = context.QPages.AsQueryable()
                    .Where(p => p.ObjectType == "Course" && p.ContentControl == "Course" && p.IsHidden == false)
                    .Select(p => p.ObjectIdentifier);

                if (filter.HasWebPage.Value)
                    query = query.Where(x => subQuery.Contains(x.CourseIdentifier));
                else
                    query = query.Where(x => !subQuery.Contains(x.CourseIdentifier));
            }

            {
                var hasPageFilter = false;
                var subQuery = context.QPages.AsQueryable()
                    .Where(p => p.ObjectType == "Course" && p.ContentControl == "Course");

                if (filter.WebPageAuthoredSince.HasValue)
                {
                    hasPageFilter = true;
                    subQuery = subQuery.Where(x => x.AuthorDate >= filter.WebPageAuthoredSince.Value);
                }

                if (filter.WebPageAuthoredBefore.HasValue)
                {
                    hasPageFilter = true;
                    subQuery = subQuery.Where(x => x.AuthorDate < filter.WebPageAuthoredBefore.Value);
                }

                if (hasPageFilter)
                    query = query.Where(x => subQuery.Any(p => p.ObjectIdentifier == x.CourseIdentifier));
            }

            return query;
        }

        internal static IQueryable<VCourse> ApplyFilter(IQueryable<VCourse> query, QCourseFilter filter, InternalDbContext context)
        {
            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogIdentifier == filter.CatalogIdentifier.Value);

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookIdentifier == filter.GradebookIdentifier.Value);
            else if (filter.HasGradebook == true)
                query = query.Where(x => x.GradebookIdentifier != null && x.GradebookIdentifier != Guid.Empty);
            else if (filter.HasGradebook == false)
                query = query.Where(x => x.GradebookIdentifier == null || x.GradebookIdentifier == Guid.Empty);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (!string.IsNullOrWhiteSpace(filter.CourseName))
                query = query.Where(x => x.CourseName.Contains(filter.CourseName));

            if (!string.IsNullOrWhiteSpace(filter.CourseLabel))
                query = query.Where(x => x.CourseLabel.Contains(filter.CourseLabel));

            if (filter.GradebookTitle.IsNotEmpty())
                query = query.Where(x => x.GradebookTitle.Contains(filter.GradebookTitle));

            if (filter.HasWebPage.HasValue)
            {
                var subQuery = context.QPages.AsQueryable()
                    .Where(p => p.ObjectType == "Course" && p.ContentControl == "Course" && p.IsHidden == false)
                    .Select(p => p.ObjectIdentifier);

                if (filter.HasWebPage.Value)
                    query = query.Where(x => subQuery.Contains(x.CourseIdentifier));
                else
                    query = query.Where(x => !subQuery.Contains(x.CourseIdentifier));
            }

            {
                var hasPageFilter = false;
                var subQuery = context.QPages.AsQueryable()
                    .Where(p => p.ObjectType == "Course" && p.ContentControl == "Course");

                if (filter.WebPageAuthoredSince.HasValue)
                {
                    hasPageFilter = true;
                    subQuery = subQuery.Where(x => x.AuthorDate >= filter.WebPageAuthoredSince.Value);
                }

                if (filter.WebPageAuthoredBefore.HasValue)
                {
                    hasPageFilter = true;
                    subQuery = subQuery.Where(x => x.AuthorDate < filter.WebPageAuthoredBefore.Value);
                }

                if (hasPageFilter)
                    query = query.Where(x => subQuery.Any(p => p.ObjectIdentifier == x.CourseIdentifier));
            }

            return query;
        }
    }
}
