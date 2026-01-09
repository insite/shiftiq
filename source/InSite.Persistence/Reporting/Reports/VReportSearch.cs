using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class VReportSearch
    {
        private class ReadHelper : ReadHelper<VReport>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VReport>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.VReports.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static IReadOnlyList<VReport> Select(
            Expression<Func<VReport, bool>> filter,
            params Expression<Func<VReport, object>>[] includes)
        {
            return ReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<VReport> Select(
            Expression<Func<VReport, bool>> filter,
            string sortExpression,
            params Expression<Func<VReport, object>>[] includes)
        {
            return ReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static VReport SelectFirst(
            Expression<Func<VReport, bool>> filter,
            params Expression<Func<VReport, object>>[] includes)
        {
            return ReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<VReport, T>> binder,
            Expression<Func<VReport, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<VReport, T>> binder,
            Expression<Func<VReport, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static ListItem[] GetCreatedByListItems(Guid organizationId, Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                return db.VReports
                    .Where(x => x.OrganizationIdentifier == organizationId && x.UserIdentifier == userId && x.CreatedBy.HasValue)
                    .Select(x => new ListItem
                    {
                        Value = x.CreatedBy.Value.ToString(),
                        Text = x.CreatedByFullName
                    })
                    .Distinct()
                    .OrderBy(x => x.Text)
                    .ToArray();
            }
        }

        public static VReport Select(Guid report)
        {
            using (var db = new InternalDbContext())
            {
                return db.VReports.FirstOrDefault(x => x.ReportIdentifier == report);
            }
        }

        public static int Count(VReportFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static List<VReport> Select(VReportFilter filter)
        {
            var sortExpression = nameof(VReport.ReportTitle);
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<VReport> CreateQuery(VReportFilter filter, InternalDbContext db)
        {
            var query = db.VReports
                .AsNoTracking()
                .AsQueryable();

            query = filter.IncludeShared
                ? query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier || x.ReportType == ReportType.Shared)
                : query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            query = ApplyPermissionFilter(filter, db, query);

            if (filter.ReportTitle.HasValue())
                query = query.Where(x => x.ReportTitle.Contains(filter.ReportTitle));

            if (filter.ReportDescription.HasValue())
                query = query.Where(x => x.ReportDescription.Contains(filter.ReportDescription));

            if (filter.ReportTypes != null && filter.ReportTypes.Length > 0)
                query = query.Where(x => filter.ReportTypes.Contains(x.ReportType));

            if (filter.IsCreator.HasValue)
                if (filter.IsCreator.Value)
                    query = query.Where(x => x.CreatedBy == filter.UserIdentifier);
                else
                    query = query.Where(x => x.CreatedBy != filter.UserIdentifier);

            if (filter.CreatedBy.HasValue)
                query = query.Where(x => x.CreatedBy == filter.CreatedBy);

            if (filter.ModifiedSince.HasValue)
                query = query.Where(x => x.Modified >= filter.ModifiedSince.Value);

            if (filter.ModifiedBefore.HasValue)
                query = query.Where(x => x.Modified < filter.ModifiedBefore.Value);

            return query;
        }

        private static IQueryable<VReport> ApplyPermissionFilter(VReportFilter filter, InternalDbContext db, IQueryable<VReport> query)
        {
            return query.Where(
                x => x.UserIdentifier == filter.UserIdentifier && x.OrganizationIdentifier == filter.OrganizationIdentifier
                ||
                db.TGroupPermissions
                    .Where(y => y.ObjectIdentifier == x.ReportIdentifier)
                    .Join(db.Memberships.Where(y => y.UserIdentifier == filter.UserIdentifier && y.Group.OrganizationIdentifier == filter.OrganizationIdentifier),
                        a => a.GroupIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => a
                    )
                    .Any()
            );
        }

        public static bool HasPermissions(VReport report, Guid organizationIdentifier, Guid userIdentifier)
        {
            if (!string.Equals(report.ReportType, ReportType.Shared, StringComparison.OrdinalIgnoreCase)
                && report.OrganizationIdentifier != organizationIdentifier
                )
            {
                return false;
            }

            if (report.UserIdentifier == userIdentifier && report.OrganizationIdentifier == organizationIdentifier)
                return true;

            using (var db = new InternalDbContext(false))
            {
                return db.TGroupPermissions
                    .Where(y => y.ObjectIdentifier == report.ReportIdentifier)
                    .Join(db.Memberships.Where(y => y.UserIdentifier == userIdentifier && y.Group.OrganizationIdentifier == organizationIdentifier),
                        a => a.GroupIdentifier,
                        b => b.GroupIdentifier,
                        (a, b) => a
                    )
                    .Any();
            }
        }
    }
}
