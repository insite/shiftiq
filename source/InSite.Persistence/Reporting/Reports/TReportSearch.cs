using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;



namespace InSite.Persistence
{
    public class TReportSearch
    {
        private class ReadHelper : ReadHelper<TReport>
        {
            public static readonly ReadHelper Instance = new ReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TReport>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TReports.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static IReadOnlyList<TReport> Select(
            Expression<Func<TReport, bool>> filter,
            params Expression<Func<TReport, object>>[] includes)
        {
            return ReadHelper.Instance.Select(filter, includes);
        }

        public static IReadOnlyList<TReport> Select(
            Expression<Func<TReport, bool>> filter,
            string sortExpression,
            params Expression<Func<TReport, object>>[] includes)
        {
            return ReadHelper.Instance.Select(filter, sortExpression, includes);
        }

        public static TReport SelectFirst(
            Expression<Func<TReport, bool>> filter,
            params Expression<Func<TReport, object>>[] includes)
        {
            return ReadHelper.Instance.SelectFirst(filter, includes);
        }

        public static IReadOnlyList<T> Bind<T>(
            Expression<Func<TReport, T>> binder,
            Expression<Func<TReport, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);
        }

        public static T BindFirst<T>(
            Expression<Func<TReport, T>> binder,
            Expression<Func<TReport, bool>> filter,
            string modelSort = null,
            string entitySort = null)
        {
            return ReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);
        }

        public static TReport Select(Guid report)
        {
            using (var db = new InternalDbContext())
            {
                return db.TReports.FirstOrDefault(x => x.ReportIdentifier == report);
            }
        }

        public static IReadOnlyList<TReport> Select(
            Guid organizationId,
            Guid userId,
            string reportType,
            string reportData,
            string reportDescription
            )
        {
            using (var db = new InternalDbContext())
            {
                return db.TReports
                    .Where(x =>
                        x.ReportType == reportType
                        && x.ReportData == reportData
                        && x.OrganizationIdentifier == organizationId
                        && x.UserIdentifier == userId
                        && x.ReportDescription == reportDescription
                    )
                    .ToList();
            }
        }
    }
}
