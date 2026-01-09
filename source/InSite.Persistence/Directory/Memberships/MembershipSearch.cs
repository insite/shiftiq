using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Persistence
{
    public static class MembershipSearch
    {
        public class RoleMembership
        {
            public string GroupName { get; set; }
            public string GroupType { get; set; }
            public string OrganizationCode { get; set; }
            public string OrganizationName { get; set; }
            public string UserFullName { get; set; }

            public Guid GroupIdentifier { get; set; }
            public Guid OrganizationIdentifier { get; set; }
            public Guid UserIdentifier { get; set; }

            public bool IsActive { get; set; }
        }

        #region Private

        private class MembershipReadHelper : ReadHelper<Membership>
        {
            public static readonly MembershipReadHelper Instance = new MembershipReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Membership>, TResult> func)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var query = db.Memberships.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(
                Expression<Func<Membership, T>> binder,
                MembershipFilter filter
                )
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var query = db.Memberships.AsQueryable().AsNoTracking();

                    IQueryable<T> bind(IQueryable<Membership> q) => q.Select(binder);

                    IQueryable<Membership> filterQuery(IQueryable<Membership> q) => CreateQuery(q, filter, db);

                    var modelQuery = BuildQuery(query, bind, filterQuery, q => q, filter.Paging, filter.OrderBy, null, false);

                    return modelQuery.ToArray();
                }
            }
        }

        #endregion

        #region SELECT (Bind)

        public static T Bind<T>(Guid group, Guid user, Expression<Func<Membership, T>> binder) =>
            MembershipReadHelper.Instance.BindFirst(binder, x => x.GroupIdentifier == group && x.UserIdentifier == user);

        public static T[] Bind<T>(
            Expression<Func<Membership, T>> binder,
            Expression<Func<Membership, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            MembershipReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static T[] Bind<T>(
            Expression<Func<Membership, T>> binder,
            Expression<Func<Membership, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null
            ) =>
            MembershipReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);

        public static T BindFirst<T>(
            Expression<Func<Membership, T>> binder,
            Expression<Func<Membership, bool>> filter,
            string modelSort = null, string entitySort = null) =>
            MembershipReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public static T[] Bind<T>(Expression<Func<Membership, T>> binder, MembershipFilter filter)
        {
            return MembershipReadHelper.Instance.Bind(binder, filter);
        }

        public static T[] Distinct<T>(
            Expression<Func<Membership, T>> binder,
            Expression<Func<Membership, bool>> filter = null,
            string modelSort = null)
        {
            return MembershipReadHelper.Instance.Distinct(binder, filter, modelSort);
        }

        public static int Count(Expression<Func<Membership, bool>> filter) =>
            MembershipReadHelper.Instance.Count(filter);

        public static bool Exists(Expression<Func<Membership, bool>> filter) =>
            MembershipReadHelper.Instance.Exists(filter);

        public static int Count(MembershipFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQuery(filter, db).Count();
        }

        public static int Count(Guid group, Guid user)
        {
            using (var db = new InternalDbContext())
            {
                return db.Memberships
                    .Count(x => x.GroupIdentifier == group && x.UserIdentifier == user);
            }
        }

        public static Membership[] Select(
            Expression<Func<Membership, bool>> filter,
            params Expression<Func<Membership, object>>[] includes) =>
            MembershipReadHelper.Instance.Select(filter, includes);

        public static Membership SelectFirst(
            Expression<Func<Membership, bool>> filter,
            params Expression<Func<Membership, object>>[] includes) =>
            MembershipReadHelper.Instance.SelectFirst(filter, includes);

        public static Membership[] Select(
            Expression<Func<Membership, bool>> filter,
            string sortExpression,
            params Expression<Func<Membership, object>>[] includes) =>
            MembershipReadHelper.Instance.Select(filter, sortExpression, includes);

        #endregion

        public static bool Exists(Guid groupId, Guid userId) =>
            MembershipReadHelper.Instance.Exists(x => x.GroupIdentifier == groupId && x.UserIdentifier == userId);

        public static bool Exists(Guid[] groups, Guid userId) =>
            MembershipReadHelper.Instance.Exists(x => groups.Any(y => y == x.GroupIdentifier) && x.UserIdentifier == userId);

        public static bool IsUserAssignedToDepartment(Guid organizationId, Guid userId) =>
            Exists(x => x.UserIdentifier == userId && x.Group.OrganizationIdentifier == organizationId && x.Group.GroupType == GroupTypes.Department);

        public static bool IsUserAssignedToRole(Guid organizationId, Guid userId) =>
            Exists(x => x.UserIdentifier == userId && x.Group.OrganizationIdentifier == organizationId && x.Group.GroupType == GroupTypes.Role);

        public static Membership[] GetUserDepartmentMemberships(Guid organizationId, Guid userId)
        {
            return Bind(
                x => x,
                x => x.UserIdentifier == userId
                  && x.Group.OrganizationIdentifier == organizationId
                  && x.Group.GroupType == GroupTypes.Department);
        }

        public static Membership Select(Guid group, Guid user, params Expression<Func<Membership, object>>[] includes)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.Memberships
                    .Where(x => x.GroupIdentifier == group && x.UserIdentifier == user)
                    .ApplyIncludes(includes);

                return query.SingleOrDefault();
            }
        }

        public static string[] SelectEmploymentTypes(Guid organizationId, bool excludeAdministration = true)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.Memberships.Where(
                    d => d.Group.GroupType == GroupTypes.Department
                      && d.Group.OrganizationIdentifier == organizationId
                      && d.MembershipType != null);

                if (excludeAdministration)
                    query = query.Where(x => x.MembershipType != "Administration");

                return query.Select(x => x.MembershipType)
                      .Distinct()
                      .OrderBy(x => x)
                      .ToArray();
            }
        }

        public static IReadOnlyList<RoleMembership> SelectMembershipDetails(int? groupKey, Guid? userKey)
        {
            const string query = @"
select G.GroupName
     , G.GroupType
     , T.CompanyName as OrganizationName
     , T.OrganizationCode
     , U.FullName    as UserFullName
     , G.GroupIdentifier
     , T.OrganizationIdentifier
     , U.UserIdentifier
from contacts.Membership               as R
     inner join contacts.QGroup        as G on G.GroupIdentifier = R.GroupIdentifier
     inner join identities.[User]      as U on U.UserIdentifier = R.UserIdentifier
     inner join accounts.QOrganization as T on T.OrganizationIdentifier = G.OrganizationIdentifier
where
    (@GroupIdentifier is null or G.GroupIdentifier = @GroupIdentifier)
    and
    (@UserIdentifier is null or U.UserIdentifier = @UserIdentifier)
order by
    G.GroupType, G.GroupName;";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<RoleMembership>(
                    query,
                    new SqlParameter("@GroupIdentifier", (object)groupKey ?? DBNull.Value),
                    new SqlParameter("@UserIdentifier", (object)userKey ?? DBNull.Value)).ToArray();
            }
        }

        private static IQueryable<Membership> CreateQuery(MembershipFilter filter, InternalDbContext db) =>
            CreateQuery(db.Memberships.AsQueryable(), filter, db);

        private static IQueryable<Membership> CreateQuery(IQueryable<Membership> query, MembershipFilter filter, InternalDbContext db)
        {
            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.Group.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.GroupName.HasValue())
                query = query.Where(x => x.Group.GroupName.Contains(filter.GroupName));

            if (filter.GroupType.HasValue())
                query = query.Where(x => x.Group.GroupType == filter.GroupType);

            if (filter.GroupLabel.HasValue())
                query = query.Where(x => x.Group.GroupLabel == filter.GroupLabel);

            if (filter.UserCode.HasValue())
                query = query.Where(x => x.User.Persons.Any(p => p.PersonCode.Contains(filter.UserCode)));

            if (filter.UserFullName.HasValue())
                query = query.Where(x => x.User.FullName.Contains(filter.UserFullName));

            if (filter.UserEmail.HasValue())
                query = query.Where(x => x.User.Email.Contains(filter.UserEmail));

            if (filter.MembershipFunctions.IsNotEmpty())
                query = query.Where(x => filter.MembershipFunctions.Any(f => f == x.MembershipType));

            if (filter.HasMembershipFunction.HasValue)
                query = query.Where(x => filter.HasMembershipFunction.Value ? x.MembershipType != null : x.MembershipType == null);

            if (filter.MembershipStatuses.IsNotEmpty())
                query = query.Where(x =>
                    db.Persons.Any(y =>
                        y.UserIdentifier == x.UserIdentifier
                        && y.OrganizationIdentifier == x.Group.OrganizationIdentifier
                        && y.MembershipStatusItemIdentifier != null
                        && filter.MembershipStatuses.Contains(y.MembershipStatusItemIdentifier.Value)
                    )
                );

            if (filter.EffectiveSince.HasValue)
                query = query.Where(x => x.Assigned >= filter.EffectiveSince);

            if (filter.EffectiveBefore.HasValue)
                query = query.Where(x => x.Assigned < filter.EffectiveBefore);

            if (filter.ExpirySince.HasValue)
                query = query.Where(x => x.MembershipExpiry >= filter.ExpirySince);

            if (filter.EffectiveBefore.HasValue)
                query = query.Where(x => x.MembershipExpiry < filter.ExpiryBefore);

            return query;
        }

        public static int CountPeople(Guid organizationId, Guid? department, string[] employmentTypes)
        {
            using (var db = new InternalDbContext())
            {
                return QueryPeople(organizationId, department, employmentTypes, db).Count();
            }
        }

        public static List<Person> QueryPeople(Guid organizationId, Guid? department, string[] employmentTypes, Paging paging)
        {
            using (var db = new InternalDbContext())
            {
                return QueryPeople(organizationId, department, employmentTypes, db)
                    .ApplyPaging(paging)
                    .ToList();
            }
        }

        private static IQueryable<Person> QueryPeople(Guid organizationId, Guid? department, string[] employmentTypes, InternalDbContext db)
        {
            IQueryable<Person> query;

            if (department == null)
            {
                query = employmentTypes.IsEmpty()
                    ? db.Persons
                        .Include(x => x.User)
                        .Where(
                            x => x.OrganizationIdentifier == organizationId
                              && x.User.Memberships.Any(y => y.GroupIdentifier == department))
                    : db.Persons
                        .Include(x => x.User)
                        .Where(
                            x => x.OrganizationIdentifier == organizationId
                              && x.User.Memberships.Any(
                                y => employmentTypes.Contains(y.MembershipType)
                                  && y.GroupIdentifier == department))
                    ;
            }
            else
            {
                var relationshipQuery = db.Memberships.Where(x => x.GroupIdentifier == department);

                if (employmentTypes.IsNotEmpty())
                    relationshipQuery = relationshipQuery.Where(x => employmentTypes.Contains(x.MembershipType));

                query = relationshipQuery
                    .SelectMany(x => x.User.Persons)
                    .Where(x => x.OrganizationIdentifier == organizationId);
            }

            return query
                .Include(x => x.User)
                .Where(x => !x.User.UtcArchived.HasValue)
                .OrderBy(x => x.User.FirstName)
                .ThenBy(x => x.User.LastName);
        }
    }
}
