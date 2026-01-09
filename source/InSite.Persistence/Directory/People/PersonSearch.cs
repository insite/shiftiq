using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;

using InSite.Application.Contacts.Read;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;

namespace InSite.Persistence
{
    public static class PersonSearch
    {
        public static readonly string[] KeywordConjunctions = new string[] { " this ", " is ", " no ", " it ", " and ", " a ", " to ", " for ", " the ", " at ", " in ", " my " };

        public static readonly char[] KeywordGarbageChars = new char[] { ';', ':', '\'', '"', '-', ',', '&', '/', '\\', '(', ')', '[', ']' };
        #region Classes
        private class PersonReadHelper : ReadHelper<Person>
        {
            public static readonly PersonReadHelper Instance = new PersonReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<Person>, TResult> func)
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    var query = db.Persons.AsNoTracking().AsQueryable();

                    return func(query);
                }
            }
        }

        public static int CountByPersonFilter(JobPersonFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var query = FilterQueryByPersonFilter(filter, db.Persons, db);

                return query.Count();
            }
        }

        public static List<Person> SelectList(JobPersonFilter filter, params Expression<Func<Person, object>>[] includes)
        {
            var sortExpression = !string.IsNullOrEmpty(filter.OrderBy)
                ? filter.OrderBy
                : "Updated DESC";

            using (var db = new InternalDbContext())
            {
                var query = FilterQueryByPersonFilter(filter, db.Persons, db).ApplyIncludes(includes);

                return query
                    .OrderBy(sortExpression)
                    .ApplyPaging(filter)
                    .ToList();
            }
        }

        private static IQueryable<Person> FilterQueryByPersonFilter(JobPersonFilter filter, IQueryable<Person> query, InternalDbContext db)
        {
            db.Database.CommandTimeout = 2 * 60; // 2 minutes

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier);

            if (filter.Cities.IsNotEmpty())
                query = query.Where(x => filter.Cities.Any(y => y == x.HomeAddress.City)
                || filter.Cities.Any(y => y == x.BillingAddress.City)
                || filter.Cities.Any(y => y == x.ShippingAddress.City)
                || filter.Cities.Any(y => y == x.WorkAddress.City));

            if (filter.City.IsNotEmpty())
                query = query.Where(x => x.HomeAddress.City.Contains(filter.City));

            if (filter.FullName.IsNotEmpty())
                query = query.Where(x => x.User.FullName.Contains(filter.FullName));

            if (filter.AreaOfInterest.HasValue)
                query = query.Where(x => x.OccupationStandardIdentifier == filter.AreaOfInterest.Value);

            if (filter.CurrentJobTitle.IsNotEmpty())
                query = query.Where(x => x.JobTitle.Contains(filter.CurrentJobTitle));

            if (filter.WillingToRelocate.HasValue)
                query = query.Where(x => x.CandidateIsWillingToRelocate == filter.WillingToRelocate.Value);

            if (filter.IsActivelySeeking)
                query = query.Where(x => x.CandidateIsActivelySeeking == true);

            if (filter.IsApproved.HasValue)
                query = filter.IsApproved == true ? query.Where(x => x.JobsApproved != null)
                    : query.Where(x => x.JobsApproved == null);

            if (filter.Occupation.IsNotEmpty())
            {
                var builder = new StringBuilder(filter.Occupation.Trim());

                foreach (var garbageChar in KeywordGarbageChars) builder.Replace(garbageChar, ' ');
                foreach (var conjunction in KeywordConjunctions) builder.Replace(conjunction, " ");
                builder.Replace("  ", " ");

                var words = builder.ToString().Split(' ');
                query = query.Where(x => x.User.PersonFields.Any(y => y.FieldName == "Industry Interest Area"
                            && (words.Any(z => y.FieldValue.Contains(z)))));
            }

            if (!string.IsNullOrEmpty(filter.Keywords))
            {
                var builder = new StringBuilder(filter.Keywords.Trim());

                foreach (var garbageChar in KeywordGarbageChars) builder.Replace(garbageChar, ' ');
                foreach (var conjunction in KeywordConjunctions) builder.Replace(conjunction, " ");
                builder.Replace("  ", " ");

                var keywords = builder.ToString().Split(new char[] { ' ', ',' });
                foreach (var keyword in keywords)
                {
                    query = query.Where(x => x.User.FirstName == keyword || x.User.LastName == keyword);
                }
            }
            return query;

        }

        #endregion

        #region PersonExamRegistrationFilter

        public static int Count(PersonExamRegistrationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var filterExpr = CreatePersonExamRegistrationFilter(filter);

                return db.Persons.Where(filterExpr).Count();
            }
        }

        public static PersonExamRegistrationItem[] Select(PersonExamRegistrationFilter filter)
        {
            using (var db = new InternalDbContext())
            {
                var filterExpr = CreatePersonExamRegistrationFilter(filter);

                return db.Persons
                    .Where(filterExpr)
                    .Select(x => new PersonExamRegistrationItem
                    {
                        UserIdentifier = x.UserIdentifier,
                        Email = x.User.Email,
                        EmailAlternate = x.User.EmailAlternate,
                        EmployerIdentifier = x.EmployerGroup.GroupIdentifier,
                        EmployerName = x.EmployerGroup.GroupName,
                        FullName = x.User.FullName,
                        PersonCode = x.PersonCode
                    })
                    .OrderBy(filter.OrderBy.IfNullOrEmpty("UserIdentifier"))
                    .ApplyPaging(filter)
                    .ToArray();
            }
        }

        private static Expression<Func<Person, bool>> CreatePersonExamRegistrationFilter(PersonExamRegistrationFilter filter)
        {
            var predicate = PredicateBuilder.False<Person>();

            if (filter.PersonCode.HasValue())
            {
                var codes = StringHelper.Split(filter.PersonCode);
                var expr = LinqExtensions1.Expr((Person x) => codes.Any(y => y == x.PersonCode));

                predicate = predicate.Or(expr.Expand());
            }

            if (filter.PersonName.IsNotEmpty() || filter.PersonEmail.IsNotEmpty() || filter.EmployerGroupIdentifier.HasValue)
            {
                var expr = PredicateBuilder.True<Person>();

                if (filter.PersonName.IsNotEmpty())
                    expr = expr.And((
                        x => x.User.FullName.Contains(filter.PersonName)
                          || x.User.FirstName.Contains(filter.PersonName)
                          || x.User.LastName.Contains(filter.PersonName)
                          || x.User.FirstName + " " + x.User.LastName == filter.PersonName));

                if (filter.PersonEmail.IsNotEmpty())
                    expr = expr.And(
                        x => (x.User.Email.Contains(filter.PersonEmail)
                           || x.User.EmailAlternate.Contains(filter.PersonEmail)));

                if (filter.EmployerGroupIdentifier.HasValue)
                    expr = expr.And(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier.Value);

                predicate = predicate.Or(expr.Expand());
            }

            Expression<Func<Person, bool>> result;

            if (filter.SavedIdentifiers.IsNotEmpty())
                result = LinqExtensions1.Expr(
                    (Person x) => x.OrganizationIdentifier == filter.OrganizationIdentifier && filter.SavedIdentifiers.Contains(x.UserIdentifier) && predicate.Invoke(x));
            else
                result = LinqExtensions1.Expr(
                    (Person x) => x.OrganizationIdentifier == filter.OrganizationIdentifier && predicate.Invoke(x));

            return result.Expand();
        }

        #endregion

        #region Select

        public static Guid GetUserIdentifier(Guid organization, string code)
        {
            using (var db = new InternalDbContext())
                return db.Persons
                    .Where(x => x.OrganizationIdentifier == organization && x.PersonCode == code)
                    .Select(x => x.UserIdentifier)
                    .FirstOrDefault();
        }

        public static Guid[] GetOrganizationIds(Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.UserIdentifier == userId)
                    .Select(x => x.OrganizationIdentifier)
                    .ToArray();
            }
        }

        public static string[] GetOrganizationNames(Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                return db.Persons
                    .Where(x => x.UserIdentifier == userId)
                    .Select(x => x.Organization.CompanyName)
                    .OrderBy(x => x)
                    .ToArray();
            }
        }

        internal static List<EmailVariables> GetEnvelopeDrafts(Guid userId)
        {
            using (var db = new InternalDbContext())
            {
                var people = db.Persons
                    .Where(x => x.UserIdentifier == userId)
                    .Select(x => new
                    {
                        RecipientCode = x.PersonCode,
                        RecipientEmail = x.User.Email,
                        RecipientEmailEnabled = x.EmailEnabled,
                        RecipientEmailAlternate = x.User.EmailAlternate,
                        RecipientEmailAlternateEnabled = x.EmailAlternateEnabled,
                        RecipientIdentifier = x.UserIdentifier,
                        RecipientName = x.FullName,
                        RecipientNameFirst = x.User.FirstName,
                        RecipientNameLast = x.User.LastName,
                        x.OrganizationIdentifier
                    })
                    .OrderBy(x => x.RecipientEmail)
                    .ThenBy(x => x.RecipientEmailAlternate)
                    .ToArray();

                var list = new List<EmailVariables>();

                foreach (var person in people)
                {
                    var email = person.RecipientEmailEnabled
                        ? person.RecipientEmail
                        : (person.RecipientEmailAlternateEnabled
                         ? person.RecipientEmailAlternate
                         : null);

                    if (email == null)
                        continue;

                    var item = new EmailVariables
                    {
                        RecipientIdentifier = person.RecipientIdentifier,
                        RecipientCode = person.RecipientCode,
                        RecipientEmail = email,
                        RecipientName = person.RecipientName,
                        RecipientNameFirst = person.RecipientNameFirst,
                        RecipientNameLast = person.RecipientNameLast,
                        OrganizationIdentifier = person.OrganizationIdentifier
                    };
                    list.Add(item);
                }

                return list;
            }
        }

        public static Person Select(Guid organization, Guid user, params Expression<Func<Person, object>>[] includes)
        {
            using (new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted }))
            {
                using (var db = new InternalDbContext())
                {
                    db.Configuration.ProxyCreationEnabled = false;

                    return db.Persons.AsNoTracking()
                        .ApplyIncludes(includes)
                        .FirstOrDefault(x => x.OrganizationIdentifier == organization && x.UserIdentifier == user);
                }
            }
        }

        public static Person SelectByemployerIdentifier(Guid? EmployerIdentifier, params Expression<Func<Person, object>>[] includes)
        {
            if (EmployerIdentifier is null) return null;
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var q = db.Persons.AsNoTracking().AsQueryable();

                if (includes.IsNotEmpty())
                    foreach (var include in includes)
                        q = q.Include(include);

                return q.FirstOrDefault(x => x.EmployerGroupIdentifier == EmployerIdentifier);
            }
        }

        public static Person Select(Guid organization, string code, params Expression<Func<Person, object>>[] includes)
        {
            using (var db = new InternalDbContext())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var q = db.Persons.AsNoTracking().AsQueryable();

                if (includes.IsNotEmpty())
                    foreach (var include in includes)
                        q = q.Include(include);

                return q.FirstOrDefault(x => x.OrganizationIdentifier == organization && x.PersonCode == code);
            }
        }

        public static Person SelectByEmail(Guid? organizationId, string email)
        {
            using (var db = new InternalDbContext())
            {
                var query = db.Persons.AsNoTracking().AsQueryable();

                if (organizationId.HasValue)
                    query = query.Where(x => x.OrganizationIdentifier == organizationId);

                return query.Where(x => x.User.Email == email).FirstOrDefault();
            }
        }

        #endregion

        public static QPersonSecret GetPersonSecret(Guid personIdentifier)
        {
            using (var db = new InternalDbContext())
                return db.QPersonSecrets.FirstOrDefault(x => x.PersonIdentifier == personIdentifier);
        }

        public static bool IsUserAssignedToOrganization(Guid user, Guid organization)
        {
            using (var db = new InternalDbContext())
                return IsUserAssignedToOrganization(db, user, organization);
        }

        internal static bool IsUserAssignedToOrganization(InternalDbContext db, Guid user, Guid organization)
        {
            if (db.Persons.Any(x => x.UserIdentifier == user && x.OrganizationIdentifier == organization))
                return true;

            if (db.Memberships.Any(x => x.UserIdentifier == user && x.Group.OrganizationIdentifier == organization))
                return true;

            return false;
        }
    }
}
