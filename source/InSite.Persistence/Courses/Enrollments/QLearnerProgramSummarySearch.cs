using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace InSite.Persistence
{
    public class QLearnerProgramSummarySearch
    {
        public void GetAsAt(Guid organization, out DateTimeOffset? from, out DateTimeOffset? thru)
        {
            using (var db = new InternalDbContext())
            {
                var stats = db.QLearnerProgramSummaries.Where(x => x.OrganizationIdentifier == organization);

                if (stats.Count() > 0)
                {
                    from = stats.Min(x => x.AsAt);
                    thru = stats.Max(x => x.AsAt);
                }
                else
                {
                    from = null;
                    thru = null;
                }
            }
        }

        public DateTimeOffset[] GetSnapshotDates(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.QLearnerProgramSummaries
                    .Where(x => x.OrganizationIdentifier == organization)
                    .Select(x => x.AsAt)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToArray();
            }
        }

        private class QLearnerProgramSummaryReadHelper : ReadHelper<QLearnerProgramSummary>
        {
            public static readonly QLearnerProgramSummaryReadHelper Instance = new QLearnerProgramSummaryReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<QLearnerProgramSummary>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.QLearnerProgramSummaries.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        public static QLearnerProgramSummary Select(Guid id)
        {
            using (var db = new InternalDbContext())
            {
                return db.QLearnerProgramSummaries.FirstOrDefault(x => x.SummaryIdentifier == id);
            }
        }

        public static QLearnerProgramSummary[] Select(DateTimeOffset? asAt, Guid? organization)
        {
            using (var db = new InternalDbContext())
            {
                var stats = db.QLearnerProgramSummaries.AsNoTracking().AsQueryable();

                if (asAt != null)
                    stats = stats.Where(x => x.AsAt == asAt);

                if (organization != null)
                    stats = stats.Where(x => x.OrganizationIdentifier == organization);

                return stats.OrderBy(x => x.AsAt).ToArray();
            }
        }

        public static int Count(QLearnerProgramSummaryFilter filter) =>
            QLearnerProgramSummaryReadHelper.Instance.Count(
                (IQueryable<QLearnerProgramSummary> query) => query.Filter(filter));

        public static IList<T> Bind<T>(
            Expression<Func<QLearnerProgramSummary, T>> binder,
            QLearnerProgramSummaryFilter filter,
            string modelSort = null, string entitySort = null)
        {
            return QLearnerProgramSummaryReadHelper.Instance.Bind(
                (IQueryable<QLearnerProgramSummary> query) => query.Select(binder),
                (IQueryable<QLearnerProgramSummary> query) => query.Filter(filter),
                modelSort, entitySort);
        }
    }
}
