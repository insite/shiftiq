using System;
using System.Linq;

using InSite.Application.Courses.Read;

using Shift.Common;
using Shift.Contract;

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

            if (filter.IsVisibleInCatalog == true)
                query = query.Where(x => x.CatalogIdentifier != null && !x.CourseIsHidden && !x.Catalog.IsHidden);
            else if (filter.IsVisibleInCatalog == false)
                query = query.Where(x => x.CatalogIdentifier == null || x.CourseIsHidden || x.Catalog.IsHidden);

            if (filter.IsRestricted.HasValue)
            {
                var restricted = context.TGroupPermissions.AsQueryable()
                    .Select(p => p.ObjectIdentifier);

                if (filter.IsRestricted.Value)
                    query = query.Where(x => restricted.Contains(x.CourseIdentifier));
                else
                    query = query.Where(x => !restricted.Contains(x.CourseIdentifier));
            }

            var permissionGroups = filter.PermissionGroupIdentifiers;
            if (permissionGroups != null && permissionGroups.Length > 0)
            {
                var permitted = context.TGroupPermissions.AsQueryable()
                    .Where(p => permissionGroups.Contains(p.GroupIdentifier))
                    .Select(p => p.ObjectIdentifier);

                query = query.Where(x => permitted.Contains(x.CourseIdentifier));
            }

            return query;
        }

        internal static IQueryable<CourseMatch> ApplyFilter(IQueryable<CourseMatch> query, QCourseFilter filter, InternalDbContext context)
        {
            if (filter.CatalogIdentifier.HasValue)
                query = query.Where(x => x.CatalogId == filter.CatalogIdentifier.Value);

            if (filter.GradebookIdentifier.HasValue)
                query = query.Where(x => x.GradebookId == filter.GradebookIdentifier.Value);
            else if (filter.HasGradebook == true)
                query = query.Where(x => x.GradebookId != null && x.GradebookId != Guid.Empty);
            else if (filter.HasGradebook == false)
                query = query.Where(x => x.GradebookId == null || x.GradebookId == Guid.Empty);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationId == filter.OrganizationIdentifier.Value);

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
                    query = query.Where(x => subQuery.Contains(x.CourseId));
                else
                    query = query.Where(x => !subQuery.Contains(x.CourseId));
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
                    query = query.Where(x => subQuery.Any(p => p.ObjectIdentifier == x.CourseId));
            }

            if (filter.IsVisibleInCatalog == true)
                query = query.Where(x => x.CatalogId != null && !x.CourseIsHidden && x.CatalogIsHidden == false);
            else if (filter.IsVisibleInCatalog == false)
                query = query.Where(x => x.CatalogId == null || x.CourseIsHidden || x.CatalogIsHidden == true);

            if (filter.IsRestricted.HasValue)
            {
                var restricted = context.TGroupPermissions.AsQueryable()
                    .Select(p => p.ObjectIdentifier);

                if (filter.IsRestricted.Value)
                    query = query.Where(x => restricted.Contains(x.CourseId));
                else
                    query = query.Where(x => !restricted.Contains(x.CourseId));
            }

            var permissionGroups = filter.PermissionGroupIdentifiers;
            if (permissionGroups != null && permissionGroups.Length > 0)
            {
                var permitted = context.TGroupPermissions.AsQueryable()
                    .Where(p => permissionGroups.Contains(p.GroupIdentifier))
                    .Select(p => p.ObjectIdentifier);

                query = query.Where(x => permitted.Contains(x.CourseId));
            }

            return query;
        }
    }
}
