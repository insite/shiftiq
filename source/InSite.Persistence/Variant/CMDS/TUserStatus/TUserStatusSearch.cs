using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence.Plugin.CMDS
{
    public class TUserStatusSearch
    {
        #region Classes

        private class TUserStatusSearchHelper : ReadHelper<TUserStatus>
        {
            public static readonly TUserStatusSearchHelper Instance = new TUserStatusSearchHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<TUserStatus>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.TUserStatuses.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #region SELECT

        public static TUserStatus SelectFirst(Expression<Func<TUserStatus, bool>> filter, params Expression<Func<TUserStatus, object>>[] includes) =>
            TUserStatusSearchHelper.Instance.SelectFirst(filter, includes);

        #endregion

        #region SELECT (TUserStatusFilter)

        public int Count(TUserStatusFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .Count();
            }
        }

        public List<TUserStatus> Select(TUserStatusFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(x => x.OrganizationName)
                    .ThenBy(x => x.DepartmentName)
                    .ThenBy(x => x.UserName)
                    .ThenBy(x => x.ItemNumber)
                    .ThenBy(x => x.ItemName)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private IQueryable<TUserStatus> CreateQuery(TUserStatusFilter filter, InternalDbContext db)
        {
            var query = db.TUserStatuses
                .AsNoTracking()
                .AsQueryable();

            if (filter == null)
                return query;

            if (!filter.Enabled)
                return query.Where(x => false);

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.UserIdentifier.IsNotEmpty())
                query = query.Where(x => filter.UserIdentifier.Contains(x.UserIdentifier));

            if (filter.AsAtRange != null)
            {
                if (filter.AsAtRange.Since.HasValue)
                    query = query.Where(x => x.AsAt >= filter.AsAtRange.Since.Value);

                if (filter.AsAtRange.Before.HasValue)
                    query = query.Where(x => x.AsAt < filter.AsAtRange.Before.Value);
            }

            if (filter.AsAt != null)
                query = query.Where(x => x.AsAt == filter.AsAt.Value);

            if (filter.OrganizationName.IsNotEmpty())
                query = query.Where(x => x.OrganizationName.Contains(filter.OrganizationName));

            if (filter.DepartmentLabel.IsNotEmpty())
                query = query.Where(x => db.Departments.Any(y => x.DepartmentIdentifier == y.DepartmentIdentifier && y.DepartmentLabel == filter.DepartmentLabel));

            if (filter.DepartmentName.IsNotEmpty())
                query = query.Where(x => x.DepartmentName.Contains(filter.DepartmentName));

            if (filter.Departments.IsNotEmpty())
                query = query.Where(x => filter.Departments.Any(y => y == x.DepartmentIdentifier));

            if (filter.DepartmentRole != null)
            {
                if (filter.DepartmentRole.Length == 0)
                    query = query.Where(x => x.DepartmentRole == null);
                else
                    query = query.Where(x => x.DepartmentRole == filter.DepartmentRole);
            }

            if (filter.UserName.IsNotEmpty())
                query = query.Where(x => x.UserName.Contains(filter.UserName));

            if (filter.ExcludeAchievementTypes.IsNotEmpty())
                query = query.Where(x => x.ListDomain != "Resource" || !filter.ExcludeAchievementTypes.Contains(x.ItemNumber));

            if (filter.ExcludeStandardTypes.IsNotEmpty())
                query = query.Where(x => x.ListDomain != "Standard" || !filter.ExcludeStandardTypes.Contains(x.ItemNumber));

            if (filter.ItemName.IsNotEmpty())
                query = query.Where(x => x.ItemName.Contains(filter.ItemName));

            if (filter.ScoreFrom.HasValue)
                query = query.Where(x => x.Score >= filter.ScoreFrom);

            if (filter.ScoreThru.HasValue)
                query = query.Where(x => x.Score <= filter.ScoreThru);

            return query;
        }

        #endregion

        public void GetAsAt(Guid organization, out DateTimeOffset? from, out DateTimeOffset? thru)
        {
            using (var db = new InternalDbContext())
            {
                var stats = db.TUserStatuses.Where(x => x.OrganizationIdentifier == organization);

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

        public DateTimeOffset? GetMostRecentSnapshotDate(Guid organization, int year, int month)
        {
            using (var db = new InternalDbContext())
            {
                var mostRecent = db.TUserStatuses
                    .Where(x => x.OrganizationIdentifier == organization && x.AsAt.Year == year && x.AsAt.Month == month)
                    .Select(x => x.AsAt)
                    .DefaultIfEmpty()
                    .Max();

                if (mostRecent == DateTimeOffset.MinValue)
                    return null;

                return mostRecent;
            }
        }

        public class OrganizationSnapshotInfo
        {
            public DateTimeOffset AsAt { get; set; }
        }

        public IEnumerable<OrganizationSnapshotInfo> GetOrganizationSnapshots(Guid organization)
        {
            using (var db = new InternalDbContext())
            {
                return db.TUserStatuses
                    .Where(x => x.OrganizationIdentifier == organization)
                    .Select(x => new OrganizationSnapshotInfo { AsAt = x.AsAt })
                    .Distinct()
                    .OrderByDescending(x => x.AsAt)
                    .ToArray();
            }
        }

        public class ChartDataItem
        {
            public DateTimeOffset AsAt { get; set; }
            public string StatisticName { get; set; }
            public decimal ComplianceScore { get; set; }
        }

        public IEnumerable<ChartDataItem> GetChartData(TUserStatusFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db)
                    .GroupBy(x => new
                    {
                        x.AsAt,
                        x.ItemName
                    })
                    .Select(x => new ChartDataItem
                    {
                        AsAt = x.Key.AsAt,
                        StatisticName = x.Key.ItemName,
                        ComplianceScore = x.Average(y => y.Score) ?? 0
                    })
                    .OrderBy(x => x.AsAt).ThenBy(x => x.StatisticName)
                    .ToArray();
            }
        }

        public static List<UserStatusAchievement> SelectUserStatusAchievement(Guid organizationId, Guid department, Guid user)
        {
            if (organizationId == Guid.Empty && department == Guid.Empty && user == Guid.Empty) return null;

            var query = @"custom_cmds.SelectUserStatusResource @OrganizationIdentifier, @DepartmentIdentifier, @UserIdentifier";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                new SqlParameter("@DepartmentIdentifier", department),
                new SqlParameter("@UserIdentifier", user)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<UserStatusAchievement>(query, sqlParameters)
                    .ToList();
        }

        public static List<UserStatusStandard> SelectUserStatusStandard(Guid organizationId, Guid department, Guid user)
        {
            if (organizationId == Guid.Empty && department == Guid.Empty && user == Guid.Empty) return null;

            var query = @"custom_cmds.SelectUserStatusStandard @OrganizationIdentifier, @DepartmentIdentifier, @UserIdentifier";

            var sqlParameters = new[]
            {
                new SqlParameter("@OrganizationIdentifier", organizationId),
                new SqlParameter("@DepartmentIdentifier", department),
                new SqlParameter("@UserIdentifier", user)
            };

            using (var db = new InternalDbContext())
                return db.Database
                    .SqlQuery<UserStatusStandard>(query, sqlParameters)
                    .ToList();
        }

        public class StatisticInfo
        {
            public int Number { get; set; }
            public string Name { get; set; }
        }

        public StatisticInfo[] SelectStatisticInfo(Guid organizationId, string listDomain)
        {
            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<StatisticInfo>(
                        "SELECT DISTINCT ItemNumber AS Number, ItemName AS Name FROM custom_cmds.TUserStatus WHERE OrganizationIdentifier = @OrganizationIdentifier AND ListDomain = @ListDomain ORDER BY ItemName;",
                        new SqlParameter("@OrganizationIdentifier", organizationId),
                        new SqlParameter("@ListDomain", listDomain))
                    .ToArray();
            }
        }

        public class UserInfo
        {
            public Guid UserIdentifier { get; set; }
            public string UserName { get; set; }
        }

        public UserInfo[] SelectUsers(Guid organizationId, Guid[] departments)
        {
            var departmentWhere = departments.IsNotEmpty()
                ? " AND DepartmentIdentifier IN (" + string.Join(",", departments.Select(x => $"'{x}'")) + ")"
                : "";

            var query = $"SELECT DISTINCT UserIdentifier, UserName FROM custom_cmds.TUserStatus WHERE OrganizationIdentifier = @OrganizationIdentifier {departmentWhere} ORDER BY UserName;";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<UserInfo>(
                        query,
                        new SqlParameter("@OrganizationIdentifier", organizationId))
                    .ToArray();
            }
        }

        public CmdsBillableUserSummary[] GetBillableUserSummaries(Guid organizationId, string classification)
        {
            var query = @"
SELECT *
    FROM custom_cmds.CmdsBillableUserSummary
    WHERE OrganizationIdentifier = @OrganizationIdentifier
      AND BillingClassification = @BillingClassification
    ORDER BY LearnerNameLast, LearnerNameFirst";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<CmdsBillableUserSummary>(
                        query,
                        new SqlParameter("@OrganizationIdentifier", organizationId),
                        new SqlParameter("@BillingClassification", classification)
                    )
                    .ToArray();
            }
        }
    }
}