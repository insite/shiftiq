using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Persistence
{
    [SuppressMessage("NDepend", "ND3101:DontUseSystemRandomForSecurityPurposes", Scope = "method", Justification = "Random number generation is not security-sensitive here, therefore weak psuedo-random numbers are permitted.")]
    public static class UserSearch
    {
        private static void ExecuteOptimisticQuery(Action<InternalDbContext> action)
        {
            var options = new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted };
            using (new TransactionScope(TransactionScopeOption.Required, options))
            {
                using (var db = new InternalDbContext(false, true))
                {
                    action.Invoke(db);
                }
            }
        }

        #region Bind

        #region Public

        public static T Bind<T>(Guid id, Expression<Func<User, T>> binder) =>
            UserReadHelper.Instance.BindFirst(binder, x => x.UserIdentifier == id);

        public static T[] Bind<T>(
            Expression<Func<User, T>> binder,
            UserFilter filter,
            string modelSort = null,
            string entitySort = null) =>
            UserReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public static List<T> BindUsers<T>(Expression<Func<User, T>> binder, Expression<Func<User, bool>> filter, string orderBy = null)
        {
            using (var db = new InternalDbContext(false, true))
            {
                var query = db.Users.AsNoTracking().AsQueryable();

                if (filter != null)
                    query = query.Where(filter);

                if (orderBy.IsNotEmpty())
                    query = query.OrderBy(orderBy);

                return query.Select(binder).ToList();
            }
        }

        public static T BindFirst<T>(
            Expression<Func<User, T>> binder,
            UserFilter filter,
            string modelSort = null,
            string entitySort = null) =>
            UserReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        #endregion

        #region Private

        private class UserReadHelper : ReadHelper<User>
        {
            public static readonly UserReadHelper Instance = new UserReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<User>, TResult> func)
            {
                using (var context = new InternalDbContext())
                {
                    context.Configuration.ProxyCreationEnabled = false;

                    var query = context.Users.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(
                Expression<Func<User, T>> binder,
                UserFilter filter,
                string modelSort = null,
                string entitySort = null)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var modelQuery = BuildQuery(
                        db.Users.AsQueryable().AsNoTracking(),
                        (IQueryable<User> q) => q.Select(binder),
                        (IQueryable<User> query) => FilterQueryByUserFilter(filter, query, db),
                        q => q,
                        null, modelSort, entitySort, false);

                    return modelQuery.ToArray();
                }
            }

            public T BindFirst<T>(
                Expression<Func<User, T>> binder,
                UserFilter filter,
                string modelSort = null,
                string entitySort = null)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var modelQuery = BuildQuery(
                        db.Users.AsQueryable().AsNoTracking(),
                        (IQueryable<User> q) => q.Select(binder),
                        (IQueryable<User> query) => FilterQueryByUserFilter(filter, query, db),
                        q => q, null, modelSort, entitySort, false);

                    return modelQuery.FirstOrDefault();
                }
            }
        }

        public static List<string> CreateUniqueEmailsForOrganization(string organizationCode, string domain, int emailCount)
        {
            var mask = organizationCode + "-" + "{0}@" + domain;
            var number = new Random().Next(10000000);
            var result = new List<string>();

            for (int i = 0; i < emailCount; i++)
            {
                var numberText = number.ToString().PadLeft(7, '0');
                var email = string.Format(mask, numberText);

                while (Exists(new UserFilter { EmailExact = email }))
                {
                    number++;
                    numberText = number.ToString().PadLeft(7, '0');
                    email = string.Format(mask, numberText);
                }

                result.Add(email);

                number++;
            }

            return result;
        }

        public static string CreateUniqueEmailFromDuplicate(string duplicateEmail)
        {
            const int MaximumDuplicates = 99;
            var count = 1;

            var email = $"duplicate-01-{duplicateEmail}";
            bool unique;

            do
            {
                var duplicate = BindFirst(x => x.UserIdentifier, new UserFilter { EmailExact = email });
                unique = duplicate == null || duplicate == Guid.Empty;

                if (!unique)
                {
                    count++;
                    var padded = StringHelper.PadLeft(count.ToString(), "0", 2);
                    email = $"duplicate-{padded}-{duplicateEmail}";
                }
            }
            while (!unique && count <= MaximumDuplicates);

            if (!unique && count > MaximumDuplicates)
                throw new Exception($"Unable to create a unique email address for {duplicateEmail}. More than {Shift.Common.Humanizer.ToQuantity(MaximumDuplicates, "user")} cannot have the same email address.");

            return email;
        }

        #endregion

        #endregion

        #region Classes

        public class FastJobConnectUser
        {
            public string PersonName { get; set; }
            public string PersonEmail { get; set; }
            public string FAST { get; set; }
            public string JobConnect { get; set; }
        }

        public class InvalidEmail
        {
            public string Email { get; set; }
            public string FullName { get; set; }
            public Guid UserIdentifier { get; set; }
            public string OrganizationName { get; set; }
        }

        public class NoOrganizationUser
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public Guid UserIdentifier { get; set; }
        }

        #endregion

        #region SELECT (UserFilter)

        public static List<Person> SelectPersons(UserFilter filter, Guid organization)
        {
            using (var db = new InternalDbContext(false))
            {
                return CreateQueryByUserFilter(filter, db)
                    .Join(db.Persons.Where(x => x.OrganizationIdentifier == organization),
                        a => a.UserIdentifier,
                        b => b.UserIdentifier,
                        (a, b) => b
                    )
                    .ToList();
            }
        }

        public static int Count(UserFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByUserFilter(filter, db).Count();
        }

        public static bool Exists(UserFilter filter)
        {
            using (var db = new InternalDbContext())
                return CreateQueryByUserFilter(filter, db).Any();
        }

        public static List<User> Select(UserFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryByUserFilter(filter, db)
                    .OrderBy(filter.OrderBy)
                    .ToList();
            }
        }

        public static SearchResultList SelectSearchResults(UserFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var list = CreateQueryByUserFilter(filter, db)
                    .Select(x => new UserSearchResultItem
                    {
                        UserIdentifier = x.UserIdentifier,
                        FullName = x.FullName,
                        Email = x.Email,
                        EmailAlternate = x.EmailAlternate,
                        EmailVerified = x.EmailVerified,
                        UserPasswordHash = x.UserPasswordHash,
                        UserLicenseAccepted = x.UserLicenseAccepted,
                        UserPasswordExpired = x.UserPasswordExpired
                    })
                    .OrderBy("FullName")
                    .ApplyPaging(filter)
                    .ToSearchResult();

                if (filter.AgentOrganizationIdentifier.HasValue)
                {
                    var items = list.GetList().Cast<UserSearchResultItem>().ToList();
                    var people = PersonCriteria.Bind(
                        x => new
                        {
                            x.UserIdentifier,
                            x.UserAccessGranted,
                            x.LastAuthenticated
                        },
                        new PersonFilter
                        {
                            OrganizationIdentifier = filter.AgentOrganizationIdentifier.Value,
                            IncludeUserIdentifiers = items.Select(x => x.UserIdentifier).ToArray()
                        });

                    foreach (var item in items)
                    {
                        var person = people.FirstOrDefault(x => x.UserIdentifier == item.UserIdentifier);
                        if (person != null)
                        {
                            item.UserAccessGranted = person.UserAccessGranted;
                            item.LastAuthenticated = person.LastAuthenticated;
                        }
                    }

                    return items.ToSearchResult();
                }

                return list;
            }
        }

        public static bool IsAccessGranted(string user, string organizationCode, string action)
        {
            using (var db = new InternalDbContext())
            {
                var organizationIdentifier = db.QOrganizations
                    .FirstOrDefault(x => x.OrganizationCode == organizationCode)?
                    .OrganizationIdentifier;

                var actionId = db.TActions
                    .Where(x => x.ActionName == action)
                    .Select(x => x.ActionIdentifier)
                    .FirstOrDefault();

                return db.TGroupPermissions
                    .Any(x => x.AllowRead
                        && x.ObjectIdentifier == actionId
                        && x.Group.OrganizationIdentifier == organizationIdentifier
                        && x.Group.VMemberships.Any(y => y.User.UserEmail == user));
            }
        }

        private static IQueryable<User> CreateQueryByUserFilter(UserFilter filter, InternalDbContext db) =>
            FilterQueryByUserFilter(filter, db.Users.AsQueryable(), db);

        private static IQueryable<User> FilterQueryByUserFilter(UserFilter filter, IQueryable<User> query, InternalDbContext db)
        {
            if (filter.IncludeNullUserName)
                query = query.Where(x => x.Email != null);

            if (filter.ContactName.IsNotEmpty())
                query = query.Where(x => x.FullName.Contains(filter.ContactName) || x.FirstName.Contains(filter.ContactName) || x.LastName.Contains(filter.ContactName));

            if (filter.EmailContains.IsNotEmpty())
                query = query.Where(x => x.Email.Contains(filter.EmailContains) || (x.EmailAlternate != null && x.EmailAlternate.Contains(filter.EmailContains)));

            if (filter.EmailExact.IsNotEmpty())
                query = query.Where(x => x.Email == filter.EmailExact);

            if (filter.EmailAlternateExact.IsNotEmpty())
                query = query.Where(x => x.EmailAlternate == filter.EmailAlternateExact);

            if (filter.IsEmailValid.HasValue)
            {
                if (filter.IsEmailValid.Value)
                    query = query.Where(x => x.Email != null && DbFunctions.Like(x.Email, Pattern.ValidEmailLike));
                else
                    query = query.Where(x => x.Email != null && !DbFunctions.Like(x.Email, Pattern.ValidEmailLike));
            }

            if (filter.EmailVerified != null)
            {
                if (filter.EmailVerified.Value)
                    query = query.Where(x => x.Email == x.EmailVerified);
                else
                    query = query.Where(x => x.Email != x.EmailVerified);
            }

            if (filter.IsLicensed.HasValue)
            {
                query = filter.IsLicensed.Value
                    ? query.Where(x => x.UserLicenseAccepted != null)
                    : query.Where(x => x.UserLicenseAccepted == null);
            }

            if (filter.AgentOrganizationIdentifier.HasValue && db != null)
            {
                var personQuery = db.Persons.Where(x => x.OrganizationIdentifier == filter.AgentOrganizationIdentifier);
                var applyPersonQuery = false;

                if (!filter.LastAuthenticated.IsEmpty)
                {
                    applyPersonQuery = true;

                    if (filter.LastAuthenticated.Since.HasValue)
                        personQuery = personQuery.Where(x => x.LastAuthenticated >= filter.LastAuthenticated.Since.Value);

                    if (filter.LastAuthenticated.Before.HasValue)
                        personQuery = personQuery.Where(x => x.LastAuthenticated < filter.LastAuthenticated.Before.Value);
                }

                if (!filter.AccessGranted.IsEmpty)
                {
                    applyPersonQuery = true;

                    if (filter.AccessGranted.Since.HasValue)
                        personQuery = personQuery.Where(x => x.UserAccessGranted >= filter.AccessGranted.Since.Value);

                    if (filter.AccessGranted.Before.HasValue)
                        personQuery = personQuery.Where(x => x.UserAccessGranted < filter.AccessGranted.Before.Value);
                }
                else if (filter.IsAccessGranted.HasValue)
                {
                    applyPersonQuery = true;

                    personQuery = personQuery.Where(x => x.UserAccessGranted.HasValue == filter.IsAccessGranted);
                }

                if (applyPersonQuery)
                    query = query.Where(x => personQuery.Where(y => y.UserIdentifier == x.UserIdentifier).Any());
            }

            if (filter.CompanyName.IsNotEmpty())
                query = query.Where(x => x.Persons.Any(y => y.Organization.CompanyName.Contains(filter.CompanyName)));

            if (filter.OrganizationStatus.IsNotEmpty())
            {
                if (filter.OrganizationStatus == "No Organization")
                    query = query.Where(x => x.Persons.Count == 0);
                else if (filter.OrganizationStatus == "Single Organization")
                    query = query.Where(x => x.Persons.Count == 1);
                else if (filter.OrganizationStatus == "Multiple Organizations")
                    query = query.Where(x => x.Persons.Count > 1);
            }

            if (filter.UserSessionStatus.IsNotEmpty())
            {
                if (filter.UserSessionStatus == "Signed In")
                    query = query.Where(x => db.TUserSessions.Any(y => y.UserIdentifier == x.UserIdentifier && y.SessionIsAuthenticated));
                else if (filter.UserSessionStatus == "Never Signed In")
                    query = query.Where(x => !db.TUserSessions.Any(y => y.UserIdentifier == x.UserIdentifier && y.SessionIsAuthenticated));
            }

            if (filter.IncludeUserIdentifiers.IsNotEmpty())
                query = query.Where(x => filter.IncludeUserIdentifiers.Contains(x.UserIdentifier));

            if (filter.ExcludeUserIdentifiers.IsNotEmpty())
                query = query.Where(x => !filter.ExcludeUserIdentifiers.Contains(x.UserIdentifier));

            if (!filter.DefaultPasswordExpired.IsEmpty)
            {
                if (filter.DefaultPasswordExpired.Since.HasValue)
                    query = query.Where(x => x.DefaultPasswordExpired >= filter.DefaultPasswordExpired.Since.Value);

                if (filter.DefaultPasswordExpired.Before.HasValue)
                    query = query.Where(x => x.DefaultPasswordExpired < filter.DefaultPasswordExpired.Before.Value);
            }

            {
                var hasMembershipFilter = false;
                var predicate = PredicateBuilder.True<Membership>();

                if (filter.MembershipGroupIdentifier.HasValue)
                {
                    predicate = predicate.And(x => x.GroupIdentifier == filter.MembershipGroupIdentifier.Value);
                    hasMembershipFilter = true;
                }

                if (filter.MembershipGroupName.IsNotEmpty())
                {
                    predicate = predicate.And(x => x.Group.GroupName == filter.MembershipGroupName);
                    hasMembershipFilter = true;
                }

                if (filter.MembershipType.IsNotEmpty())
                {
                    if (filter.MembershipTypeAnd)
                    {
                        predicate = predicate.And(x => x.MembershipType == filter.MembershipType);
                    }
                    else
                    {
                        // REVIEW: Is this a bug? It produces unexpected results in the E03 partition
                        // on this form - ui/admin/learning/programs/enrollments/assign.
                        predicate = predicate.Or(x => x.MembershipType == filter.MembershipType);
                    }
                    hasMembershipFilter = true;
                }

                if (hasMembershipFilter)
                    query = query.Where(LinqExtensions1.Expr((User x) => x.Memberships.Any(y => predicate.Invoke(y))).Expand());
            }

            if (filter.IsCmds.HasValue)
                query = query.Where(x => x.AccessGrantedToCmds == filter.IsCmds);

            {
                var hasPersonFilter = false;
                var predicate = PredicateBuilder.True<Person>();

                if (filter.PersonOrganizationIdentifiers.IsNotEmpty())
                {
                    predicate = predicate.And(x => filter.PersonOrganizationIdentifiers.Contains(x.OrganizationIdentifier));
                    hasPersonFilter = true;
                }

                if (filter.PersonEmailEnabled.HasValue)
                {
                    predicate = predicate.And(x => x.EmailEnabled == filter.PersonEmailEnabled.Value);
                    hasPersonFilter = true;
                }

                if (hasPersonFilter)
                    query = query.Where(LinqExtensions1.Expr((User x) => x.Persons.Any(y => predicate.Invoke(y))).Expand());
            }

            return query;
        }

        #endregion

        #region SELECT (Fast Job Connect Users Report)

        public static IReadOnlyList<FastJobConnectUser> SelectFastJobConnectUsers()
        {
            const string query = @"
WITH CTE
  AS (
     SELECT UserFullName as PersonName
       , UserEmail as PersonEmail
       , CASE
             WHEN GroupName = 'FAST Clients' THEN
                 CONVERT(VARCHAR, RoleAssigned, 1)
             ELSE
                 NULL
         END AS FAST
       , CASE
             WHEN GroupName = 'JobConnect Candidates' THEN
                 CONVERT(VARCHAR, RoleAssigned, 1)
             ELSE
                 NULL
         END AS JobConnect
     FROM contacts.RoleSummary
     WHERE GroupName IN ( 'FAST Clients', 'JobConnect Candidates' )
     )
SELECT PersonName
  , PersonEmail
  , MAX(CTE.FAST)       AS FAST
  , MAX(CTE.JobConnect) AS JobConnect
FROM CTE
GROUP BY PersonName
  , PersonEmail
HAVING MAX(CTE.FAST) IS NOT NULL
       AND MAX(CTE.JobConnect) IS NOT NULL
ORDER BY PersonName
  , PersonEmail;";

            using (var db = new InternalDbContext())
            {
                return db.Database.SqlQuery<FastJobConnectUser>(query)
                    .ToArray();
            }
        }

        #endregion

        #region SELECT

        public static string GetFullName(Guid? user)
        {
            if (user == null || user == Guid.Empty)
                return UserNames.Someone;

            string name = null;

            ExecuteOptimisticQuery(db =>
            {
                name = db.Users.AsNoTracking()
                    .Where(x => x.UserIdentifier == user)
                    .Select(x => x.FullName)
                    .FirstOrDefault();
            });

            return name ?? UserNames.Someone;
        }

        public static string GetTimestampHtml(Guid who, string what, string verb = null, DateTimeOffset? when = null) =>
            GetTimestampHtml(GetFullName(who), what, verb, when);

        public static string GetTimestampHtml(string who, string what, string verb = null, DateTimeOffset? when = null)
        {
            if (what.IsNotEmpty())
            {
                what = what.TrimEnd(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                what = Shift.Common.Humanizer.TitleCase(what) + " ";
            }

            if (verb.IsNotEmpty())
                verb += " ";

            var whenValue = when == null || when == DateTimeOffset.MinValue
                ? "some time ago"
                : Shift.Common.Humanizer.Humanize(when);

            return what + verb + "by " + who.IfNullOrEmpty(UserNames.Someone) + " " + whenValue;
        }

        public static string GetTimestampHtml(Guid? createdby, DateTimeOffset? created, Guid? modifiedby, DateTimeOffset? modified, string noun = null)
        {
            return created.HasValue && (!modified.HasValue || created.Value == modified.Value)
                ? $"{Shift.Common.Humanizer.TitleCase(noun)} created by {GetFullName(createdby ?? Guid.Empty)} {Shift.Common.Humanizer.Humanize(created)}"
                : modified.HasValue
                    ? $"{Shift.Common.Humanizer.TitleCase(noun)} changed by {GetFullName(modifiedby ?? Guid.Empty)} {Shift.Common.Humanizer.Humanize(modified)}"
                    : null;
        }

        public static string GetTimestampHtml(string what, DateTimeOffset? when, string who, string timeZone = null)
        {
            if (when != null)
            {
                var whenText = TimeZones.Format(when.Value, timeZone);

                return Shift.Common.Humanizer.SentenceCase(what.TrimEnd(new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }))
                    + $" <span title='{whenText}'>" + Shift.Common.Humanizer.Humanize(when) + "</span>"
                    + " by " + who;
            }

            return "-";
        }

        public static List<User> SelectGroupMembers(Guid organization, Guid? department, string[] employmentTypes, Paging paging)
        {
            using (var db = new InternalDbContext())
                return GetGroupMembersQuery(db, organization, department, employmentTypes)
                    .OrderBy(x => x.FullName)
                    .ApplyPaging(paging)
                    .ToList();
        }

        public static int CountGroupMembers(Guid organization, Guid? department, string[] employmentTypes)
        {
            using (var db = new InternalDbContext())
                return GetGroupMembersQuery(db, organization, department, employmentTypes).Count();
        }

        private static IQueryable<User> GetGroupMembersQuery(InternalDbContext db, Guid organization, Guid? department, string[] employmentTypes)
        {
            IQueryable<User> query;

            if (department == null)
            {
                query = employmentTypes.IsEmpty()
                    ? db.Users.Where(
                        x => x.Memberships.Any(
                            y => y.Group.GroupType == GroupTypes.Department
                              && y.Group.OrganizationIdentifier == organization))
                    : db.Users.Where(
                        x => x.Memberships.Any(
                            y => y.Group.GroupType == GroupTypes.Department
                              && y.Group.OrganizationIdentifier == organization && employmentTypes.Contains(y.MembershipType)));
            }
            else
            {
                var relationshipQuery = db.Memberships.Where(x => x.GroupIdentifier == department);

                if (employmentTypes.IsNotEmpty())
                    relationshipQuery = relationshipQuery.Where(x => employmentTypes.Contains(x.MembershipType));

                query = relationshipQuery.Select(x => x.User);
            }

            return query.Where(x => !x.UtcArchived.HasValue);
        }

        public static SearchResultList SelectSearchResults(string sort, Paging paging, Guid organizationId)
        {
            if (string.IsNullOrEmpty(sort))
                sort = "FullName";

            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => new
                    {
                        x.UserIdentifier,
                        x.PersonCode,
                        RoleCount = x.User.Memberships.Count(),
                        x.User.Email,
                        x.EmailEnabled,
                        x.JobTitle,
                        x.ModifiedBy,
                        x.User.FullName,
                        x.Phone,
                        x.Modified
                    })
                    .OrderBy(sort)
                    .ApplyPaging(paging)
                    .ToSearchResult();
            }
        }

        public static int CountMembers(Guid[] organizations, Guid user, string groupSubType)
        {
            using (var db = new InternalDbContext())
            {
                return CreateQueryForMembers(organizations, user, groupSubType, db).Count();
            }
        }

        public static SearchResultList SelectMembers(Guid[] organizations, Guid user, string groupSubType, Paging paging, string sortExpression)
        {
            if (string.IsNullOrEmpty(sortExpression))
                sortExpression = "Name";

            using (var db = new InternalDbContext())
            {
                return CreateQueryForMembers(organizations, user, groupSubType, db)
                    .Select(x => new
                    {
                        ID = x.GroupIdentifier,
                        Thumbprint = x.Group.GroupIdentifier,
                        Name = x.Group.GroupName,
                        SubType = x.Group.GroupType,
                        MembershipType = x.MembershipType
                    })
                    .OrderBy(sortExpression)
                    .ApplyPaging(paging)
                    .ToSearchResult();
            }
        }

        private static IQueryable<Membership> CreateQueryForMembers(Guid[] organizations, Guid user, string groupSubType, InternalDbContext db)
        {
            var query = db.Memberships.Where(x => x.UserIdentifier == user && organizations.Contains(x.Group.OrganizationIdentifier));

            if (groupSubType.IsNotEmpty())
                query = query.Where(x => x.Group.GroupType == groupSubType);

            return query;
        }

        public static User Select(Guid id, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.UserIdentifier == id, includes);

        public static User SelectByAccountNumber(string number, Guid organizationId, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.Persons.Any(y => y.OrganizationIdentifier == organizationId && y.PersonCode == number), includes);

        public static User SelectByEmail(string userEmail, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.Email == userEmail, includes);

        public static User SelectByName(string name, Guid organizationId, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.FullName == name && x.Persons.Any(y => y.OrganizationIdentifier == organizationId), includes);

        public static IReadOnlyList<User> SelectByFullName(string name, Guid organizationId, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.Select(x => x.FullName.Contains(name) && x.Persons.Any(y => y.OrganizationIdentifier == organizationId), includes);

        public static User SelectByThumbprint(Guid contactIdentifier, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.UserIdentifier == contactIdentifier, includes);

        public static User SelectByName(string first, string last, Guid organizationId, params Expression<Func<User, object>>[] includes) =>
            UserReadHelper.Instance.SelectFirst(x => x.FirstName == first && x.LastName == last && x.Persons.Any(y => y.OrganizationIdentifier == organizationId), includes);

        public static bool WebContactExist(Guid user, Guid organization)
        {
            bool exists = false;

            ExecuteOptimisticQuery(db =>
            {
                exists = db.Persons.AsNoTracking()
                    .Where(c => c.OrganizationIdentifier == organization)
                    .Where(c => c.UserIdentifier == user)
                    .Any();

            });

            return exists;
        }

        public static UserModel SelectWebContact(string emailOrPersonCode, Guid organization, bool throwError = true)
        {
            // By Code
            var modelByCode = SelectWebContactByFilter(x => x.PersonCode == emailOrPersonCode && x.OrganizationIdentifier == organization);

            if (modelByCode != null)
                return modelByCode;

            // By Email
            var modelByEmail = SelectWebContactByFilter(x => x.User.Email == emailOrPersonCode && x.OrganizationIdentifier == organization);

            if (modelByEmail == null && throwError)
                throw new UserNotFoundException($"{organization} has no user account with access granted to {emailOrPersonCode}");

            return modelByEmail;
        }

        public static UserModel SelectWebContact(Guid user, Guid organization)
        {
            var model = SelectWebContactByFilter(x => x.OrganizationIdentifier == organization && x.UserIdentifier == user);

            if (model == null)
                throw new UserNotFoundException($"{organization} has no user account with access granted to {user}");

            return model;
        }

        private static UserModel SelectWebContactByFilter(Expression<Func<Person, bool>> filter)
        {
            UserModel model = null;

            ExecuteOptimisticQuery(db =>
            {
                var query = db.Persons
                    .AsNoTracking()
                    .Where(filter)
                    .Select(x => new UserModel
                    {
                        AccessGrantedToCmds = x.User.AccessGrantedToCmds,
                        Email = x.User.Email,
                        EmailVerified = x.User.EmailVerified,
                        FirstName = x.User.FirstName,
                        FullName = x.User.FullName,
                        Identifier = x.User.UserIdentifier,
                        IsCloaked = x.User.AccountCloaked.HasValue,
                        JobTitle = x.JobTitle,
                        LastName = x.User.LastName,
                        MultiFactorAuthentication = x.User.MultiFactorAuthentication,
                        PasswordExpiry = x.User.UserPasswordExpired,
                        PasswordHash = x.User.UserPasswordHash,
                        PersonCode = x.PersonCode,
                        Phone = x.Phone,
                        TimeZoneId = x.User.TimeZone,
                        UserLicenseAccepted = x.User.UserLicenseAccepted
                    });

                model = query.FirstOrDefault();

                if (model == null)
                    return;

                model.TimeZone = TimeZoneInfo.FindSystemTimeZoneById(model.TimeZoneId);

                var mfa = db.TUserAuthenticationFactors.FirstOrDefault(x => x.UserIdentifier == model.UserIdentifier);
                model.ActiveOtpMode = mfa?.OtpMode ?? OtpModes.None;
            });

            return model;
        }

        public static bool IsEmailDuplicate(Guid userId, string email)
        {
            bool exists = false;

            ExecuteOptimisticQuery(db =>
            {
                var users = db.Users.AsNoTracking().AsQueryable();

                if (userId != Guid.Empty)
                    users = users.Where(x => x.UserIdentifier != userId);

                users = users.Where(x => x.Email == email);

                exists = users.Any();
            });

            return exists;
        }

        public static AuthenticationResult ValidateUser(string username, string password, out User user)
        {
            user = null;

            if (string.IsNullOrEmpty(username))
                return AuthenticationResult.InvalidEmail;

            using (var db = new InternalDbContext())
            {
                user = db.Users.Where(x => x.Email == username && x.UserPasswordHash != null).FirstOrDefault();
                if (user == null)
                    return AuthenticationResult.InvalidEmail;
            }

            if (!PasswordHash.ValidatePassword(password, user.UserPasswordHash))
                return AuthenticationResult.InvalidPassword;

            return AuthenticationResult.Success;
        }

        #endregion

        #region Load (RespondentModel)

        public static bool LoadRespondentModel(Guid user, out RespondentModel model)
        {
            model = Bind(user, RespondentModel.BinderExpression);

            return model != null;
        }

        #endregion

        #region Tax Form Info

        public static IReadOnlyList<UserRegistrationDetail> SelectUserT2202Detail(Guid organizationId, int eventStartYear)
        {
            using (var db = new InternalDbContext())
            {
                return db.UserRegistrationDetails
                    .Where(x =>
                        x.OrganizationIdentifier == organizationId
                        && x.EventScheduledStart.Year == eventStartYear
                        && (x.AchievementLabel == null || x.AchievementLabel != "Certificate")
                        && (x.ApprovalStatus == null || x.ApprovalStatus == "Registered")
                        && x.IncludeInT2202
                    )
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.EventScheduledStart)
                    .ToList();
            }
        }

        #endregion

        #region No Organization Users

        public static Guid[] GetOrphanUsers()
        {
            const string query = @"
select U.UserIdentifier
from identities.[User] as U
where not exists
(
    select *
    from contacts.Person as P
    where P.UserIdentifier = U.UserIdentifier
)
order by U.FullName
";

            using (var db = new InternalDbContext())
            {
                return db.Database
                    .SqlQuery<Guid>(query)
                    .ToArray();
            }
        }

        #endregion

        #region Jobs

        private static readonly Func<Person, bool>[] CompletionProfileChecks = new Func<Person, bool>[]
        {
            (c) => c.User.FirstName.IsNotEmpty(),
            (c) => c.User.LastName.IsNotEmpty(),
            (c) => c.Birthdate != null,
            (c) => c.User.Email.IsNotEmpty(),
            (c) => c.Phone.IsNotEmpty() || c.User.PhoneMobile.IsNotEmpty(),
            (c) => (c.HomeAddress?.City).IsNotEmpty(),
            (c) => c.ImmigrationNumber.IsNotEmpty(),
            (c) => c.CandidateIsWillingToRelocate != null,
            (c) => c.ImmigrationLandingDate != null
        };

        public static int? GetCompletionProfilePercent(Guid organizationId, Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                var contact = db.Persons.FirstOrDefault(x => x.OrganizationIdentifier == organizationId && x.UserIdentifier == userId);
                if (contact == null)
                    return null;

                var checksCompleted = CompletionProfileChecks.Where(f => f(contact)).Count();

                return (100 * checksCompleted) / CompletionProfileChecks.Length;
            }
        }

        public static string GetCompletionStatus(int percent)
        {
            if (percent <= 30) return "danger";
            if (percent < 70) return "warning";
            return "success";
        }

        #endregion
    }
}
