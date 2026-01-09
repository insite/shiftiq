using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

using InSite.Application.Organizations.Read;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class OrganizationSearch : IOrganizationSearch
    {
        #region Classes

        public class OrganizationHierarchyInfo
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid? ParentOrganizationIdentifier { get; set; }
            public string PathCode { get; set; }
            public int PathDepth { get; set; }
            public string OrganizationName { get; set; }
            public DateTimeOffset? AccountClosed { get; set; }
            public string PathIndent { get; set; }

            public OrganizationHierarchyInfo Parent { get; set; }
            public List<OrganizationHierarchyInfo> Children { get; } = new List<OrganizationHierarchyInfo>();
        }

        #endregion

        private static readonly ConcurrentDictionary<Guid, OrganizationState> Cache
            = new ConcurrentDictionary<Guid, OrganizationState>();

        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Cache

        public OrganizationState GetModel(Guid organization)
        {
            return Select(organization);
        }

        public static string GetPersonFullNamePolicy(Guid organization)
        {
            var model = Select(organization);

            return model?.Toolkits?.Contacts?.FullNamePolicy;
        }

        public static OrganizationState Select(Guid id)
        {
            if (Cache.TryGetValue(id, out var state))
                return state;

            return null;
        }

        public static OrganizationState Select(string code)
        {
            if (code.IsEmpty())
                return null;

            code = code.ToLower();

            return Cache.Values.FirstOrDefault(x => x.Code == code);
        }

        #endregion

        #region Database

        public static int Count(OrganizationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        public static string CreateNewOrganizationCode(string acronym)
        {
            var all = SelectAll();

            var code = StringHelper.RemoveNonAlphanumericCharacters(acronym).ToLower();
            var i = 2;
            while (all.Any(x => x.Code == code))
                code += i++;

            return code;
        }

        public static bool Exists(string code)
        {
            var filter = new OrganizationFilter { OrganizationCode = code };

            return Count(filter) > 0;
        }

        public QOrganization Get(Guid organization)
        {
            using (var db = CreateContext())
                return db.QOrganizations.SingleOrDefault(x => x.OrganizationIdentifier == organization);
        }

        public static void Refresh()
        {
            Cache.Clear();

            using (var db = new InternalDbContext())
            {
                var entities = db.Organizations.AsNoTracking().ToList();

                foreach (var entity in entities)
                {
                    var organization = OrganizationAdapter.CreatePacket(entity);

                    Cache.TryAdd(organization.Identifier, organization);
                }
            }
        }

        public static void Refresh(Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.Organizations.FirstOrDefault(x => x.OrganizationIdentifier == organizationId);

                var model = OrganizationAdapter.CreatePacket(entity);

                if (Cache.TryRemove(organizationId, out _))
                    Cache.TryAdd(organizationId, model);
            }
        }

        public static List<OrganizationState> Search(OrganizationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var query = CreateQuery(filter, db).AsNoTracking();

                query = filter.OrderBy.IsNotEmpty()
                    ? query.OrderBy(filter.OrderBy)
                    : query.OrderBy(x => x.CompanyName);

                var list = query
                    .ApplyPaging(filter)
                    .ToList()
                    .Select(x => OrganizationAdapter.CreatePacket(x))
                    .ToList();

                foreach (var item in list)
                    if (item.AccountStatus == AccountStatus.Opened && item.AccountClosed.HasValue)
                        item.AccountStatus = AccountStatus.Closed;

                return list;
            }
        }

        public static List<OrganizationState> SelectAll()
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations
                    .OrderBy(x => x.CompanyName)
                    .ToList()
                    .Select(x => OrganizationAdapter.CreatePacket(x))
                    .ToList();
            }
        }

        public static List<QOrganization> SelectAllWithOrientations()
        {
            const string query = @"
SELECT t.* FROM accounts.QOrganization AS t WHERE
    EXISTS
    (
        SELECT
            *
        FROM
            achievements.QAchievement AS a
        WHERE
            a.OrganizationIdentifier = t.OrganizationIdentifier
            AND a.AchievementLabel = 'Orientation'
    )";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<QOrganization>(query)
                    .ToList();
            }
        }

        public static List<OrganizationState> SelectAllWithRoles()
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations
                    .Where(x => x.Groups.Count(y => y.GroupType == GroupTypes.Role) > 0)
                    .Where(x => x.AccountClosed == null)
                    .OrderBy(x => x.CompanyName)
                    .ToList()
                    .Select(x => OrganizationAdapter.CreatePacket(x))
                    .ToList();
            }
        }

        public static VOrganization SelectEntity(Guid key)
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations.FirstOrDefault(x => x.OrganizationIdentifier == key);
            }
        }

        public static OrganizationHierarchyInfo SelectHierarchyDescendents(Guid organization)
        {
            const string query = @"
SELECT
    Organizations.*
FROM
    accounts.QOrganizationHierarchy AS [Root]
    INNER JOIN accounts.QOrganizationHierarchy AS Organizations ON Organizations.PathCode = [Root].PathCode OR Organizations.PathCode LIKE [Root].PathCode + '/%'
WHERE
    [Root].OrganizationIdentifier = @OrganizationIdentifier;";

            using (var db = new InternalDbContext())
            {
                var dataItems = db.Database.SqlQuery<OrganizationHierarchyInfo>(
                    query,
                    new SqlParameter("@OrganizationIdentifier", organization)).ToArray();
                var itemsIndex = dataItems.ToDictionary(x => x.OrganizationIdentifier);

                OrganizationHierarchyInfo result = null;

                foreach (var item in dataItems)
                {
                    if (item.ParentOrganizationIdentifier.HasValue && itemsIndex.TryGetValue(item.ParentOrganizationIdentifier.Value, out var parent))
                    {
                        parent.Children.Add(item);
                        item.Parent = parent;
                    }
                    else
                    {
                        result = item;
                    }
                }

                return result;
            }
        }

        private static IQueryable<VOrganization> CreateQuery(OrganizationFilter filter, InternalDbContext db)
        {
            var query = db.Organizations
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.CompanyDomain))
                query = query.Where(x =>
                    x.CompanyDomain.IndexOf(filter.CompanyDomain) >= 0);

            if (!string.IsNullOrEmpty(filter.OrganizationCode))
                query = query.Where(x =>
                    x.OrganizationCode.IndexOf(filter.OrganizationCode) >= 0);

            if (!string.IsNullOrEmpty(filter.CompanyName))
                query = query.Where(x =>
                    x.CompanyName != null &&
                    x.CompanyName.IndexOf(filter.CompanyName) >= 0 ||
                    x.CompanyName.IndexOf(filter.CompanyName) >= 0);

            if (filter.IncludeOrganizationCode.IsNotEmpty())
                query = query.Where(x => filter.IncludeOrganizationCode.Contains(x.OrganizationCode));

            if (filter.ExcludeOrganizationCode.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeOrganizationCode.Contains(x.OrganizationCode));

            if (filter.IsClosed.HasValue)
                query = query.Where(x => x.AccountClosed.HasValue == filter.IsClosed.Value);

            if (filter.OrganizationIdentifiers != null && filter.OrganizationIdentifiers.Length > 0)
                query = query.Where(x => filter.OrganizationIdentifiers.Contains(x.OrganizationIdentifier));

            return query;
        }

        #endregion
    }
}