using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;

using InSite.Application.Organizations.Read;
using InSite.Domain.Organizations;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public class OrganizationSearch : IOrganizationSearch
    {
        internal InternalDbContext CreateContext() => new InternalDbContext(false);

        #region Cache

        private interface IReadOnlySnapshot
        {
            IReadOnlyDictionary<Guid, OrganizationState> ById { get; }
            IReadOnlyDictionary<string, OrganizationState> ByCode { get; }
        }

        private sealed class Snapshot : IReadOnlySnapshot
        {
            public Dictionary<Guid, OrganizationState> ById { get; }
            public Dictionary<string, OrganizationState> ByCode { get; }

            IReadOnlyDictionary<Guid, OrganizationState> IReadOnlySnapshot.ById => ById;
            IReadOnlyDictionary<string, OrganizationState> IReadOnlySnapshot.ByCode => ByCode;

            public Snapshot() : this(0) { }

            public Snapshot(int capacity)
            {
                ById = new Dictionary<Guid, OrganizationState>(capacity);
                ByCode = new Dictionary<string, OrganizationState>(capacity, StringComparer.OrdinalIgnoreCase);
            }

            public Snapshot(Snapshot source)
            {
                ById = new Dictionary<Guid, OrganizationState>(source.ById);
                ByCode = new Dictionary<string, OrganizationState>(source.ByCode, source.ByCode.Comparer);
            }
        }

        private static IReadOnlySnapshot _snapshot = new Snapshot();
        private static readonly object _snapshotWriteLock = new object();

        private static IReadOnlySnapshot GetSnapshot() => Volatile.Read(ref _snapshot);

        public OrganizationState GetModel(Guid organization)
        {
            return Select(organization);
        }

        public static string GetPersonFullNamePolicy(Guid organization)
        {
            var snapshot = GetSnapshot();

            return snapshot.ById.TryGetValue(organization, out var state)
                ? state.Toolkits?.Contacts?.FullNamePolicy
                : null;
        }

        public static OrganizationState Select(Guid id)
        {
            var snapshot = GetSnapshot();

            return snapshot.ById.TryGetValue(id, out var state)
                ? state.CloneJson()
                : null;
        }

        public static OrganizationState Select(string code)
        {
            if (code.IsEmpty())
                return null;

            var snapshot = GetSnapshot();

            return snapshot.ByCode.TryGetValue(code, out var state)
                ? state.CloneJson()
                : null;
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

        public bool CodeExists(string code, Guid? excludeOrganization = null)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.QOrganizations.AsQueryable().Where(x => x.OrganizationCode == code);

                if (excludeOrganization.HasValue)
                    query = query.Where(x => x.OrganizationIdentifier != excludeOrganization.Value);

                return query.Any();
            }
        }

        public static void Refresh()
        {
            lock (_snapshotWriteLock)
            {
                List<VOrganization> entities;
                using (var db = new InternalDbContext())
                    entities = db.Organizations.AsNoTracking().ToList();

                var snapshot = new Snapshot(entities.Count);

                foreach (var entity in entities)
                {
                    var model = OrganizationAdapter.CreatePacket(entity);

                    snapshot.ById[model.Identifier] = model;
                    snapshot.ByCode[model.Code] = model;
                }

                Volatile.Write(ref _snapshot, snapshot);
            }
        }

        public static void Refresh(Guid organizationId)
        {
            lock (_snapshotWriteLock)
            {
                VOrganization entity;
                using (var db = new InternalDbContext())
                    entity = db.Organizations.FirstOrDefault(x => x.OrganizationIdentifier == organizationId);

                if (entity == null)
                    throw ApplicationError.Create("Organization not found: {0}", organizationId);

                var snapshot = new Snapshot((Snapshot)GetSnapshot());

                if (snapshot.ById.TryGetValue(organizationId, out var cachedOrg))
                    snapshot.ByCode.Remove(cachedOrg.Code);

                var model = OrganizationAdapter.CreatePacket(entity);

                snapshot.ById[model.Identifier] = model;
                snapshot.ByCode[model.Code] = model;

                Volatile.Write(ref _snapshot, snapshot);
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

        public static List<QOrganization> SelectProjections()
        {
            using (var db = new InternalDbContext())
            {
                return db.QOrganizations
                    .AsNoTracking()
                    .OrderBy(x => x.CompanyName)
                    .ToList();
            }
        }

        public static List<QOrganization> SelectProjectionsWithOrientations()
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

        public static VOrganization SelectModel(Guid key)
        {
            using (var db = new InternalDbContext())
            {
                return db.Organizations.FirstOrDefault(x => x.OrganizationIdentifier == key);
            }
        }

        private static IQueryable<VOrganization> CreateQuery(OrganizationFilter filter, InternalDbContext db)
        {
            var query = db.Organizations
                .AsNoTracking()
                .AsQueryable();

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
