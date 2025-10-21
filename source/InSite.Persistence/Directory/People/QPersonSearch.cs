using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;
using InSite.Domain.Contacts;
using InSite.Persistence.Foundation;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Sdk.UI;

namespace InSite.Persistence
{
    public class QPersonSearch : IPersonSearch
    {
        public List<PersonName> GetNames(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .Where(x => x.OrganizationIdentifier == organizationId)
                    .Select(x => new PersonName
                    {
                        OrganizationId = organizationId,
                        PersonId = x.PersonIdentifier,
                        UserId = x.UserIdentifier,

                        First = x.User.FirstName,
                        Last = x.User.LastName,

                        FullPerson = x.FullName,
                        FullUser = x.User.FullName
                    })
                    .ToList();
            }
        }

        public QPerson GetPerson(Guid personId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.PersonIdentifier == personId)
                    .FirstOrDefault();
            }
        }

        public QPerson GetPerson(Guid userId, Guid organizationId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .FirstOrDefault();
            }
        }

        public QPerson GetPerson(Guid organizationId, string email, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.User.Email == email && x.OrganizationIdentifier == organizationId)
                    .FirstOrDefault();
            }
        }

        public QPerson GetPerson(string personCode, Guid organizationId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.PersonCode == personCode && x.OrganizationIdentifier == organizationId)
                    .FirstOrDefault();
            }
        }

        public List<QPerson> GetPersonsByEmployer(Guid employerGroupId)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .Where(x => x.EmployerGroupIdentifier == employerGroupId)
                    .ToList();
            }
        }

        public List<QPerson> GetPersonsByEmails(IEnumerable<string> emails, Guid organizationId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.OrganizationIdentifier == organizationId && emails.Contains(x.User.Email))
                    .ToList();
            }
        }

        public List<QPerson> GetPersonsByAlternateEmails(IEnumerable<string> alternateEmails, Guid organizationId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.OrganizationIdentifier == organizationId && alternateEmails.Contains(x.User.EmailAlternate))
                    .ToList();
            }
        }

        public List<QPerson> GetPersonsByPersonCodes(IEnumerable<string> personCodes, Guid organizationId, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .AsNoTracking()
                    .ApplyIncludes(includes)
                    .Where(x => x.OrganizationIdentifier == organizationId && personCodes.Contains(x.PersonCode))
                    .ToList();
            }
        }

        public QPersonAddress GetPersonAddress(Guid addressId)
        {
            using (var db = CreateContext())
            {
                return db.QPersonAddresses
                    .AsNoTracking()
                    .Where(x => x.AddressIdentifier == addressId)
                    .FirstOrDefault();
            }
        }

        public bool IsPersonExist(Guid userId, Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .Any();
            }
        }

        public bool IsPersonExist(Guid organizationId, string personCode, Guid? exceptUserId = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QPersons.AsQueryable().Where(x => x.OrganizationIdentifier == organizationId && x.PersonCode == personCode);

                if (exceptUserId.HasValue)
                    query = query.Where(x => x.UserIdentifier != exceptUserId.Value);

                return query.Any();
            }
        }

        public bool IsPersonExist(QPersonFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db).Any();
            }
        }

        public List<VDevPerson> GetDevPersons()
        {
            using (var db = CreateContext())
                return db.VDevPersons.AsNoTracking().ToList();
        }

        public List<string> GetJobDivisions(Guid organizationId)
        {
            using (var db = CreateContext())
            {
                return db.QPersons
                    .Where(x => x.OrganizationIdentifier == organizationId && x.JobDivision != null)
                    .Select(x => x.JobDivision)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }

        public List<string> GetPersonCodes(Guid organizationId, string[] codes = null)
        {
            using (var db = CreateContext())
            {
                var query = db.QPersons.AsQueryable().Where(x => x.OrganizationIdentifier == organizationId);

                if (codes.IsNotEmpty())
                    query = query.Where(x => codes.Contains(x.PersonCode));

                return query.Select(x => x.PersonCode).ToList();
            }
        }

        public int CountPersons(QPersonFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db).Count();
            }
        }

        public List<QPerson> GetPersons(QPersonFilter filter, params Expression<Func<QPerson, object>>[] includes)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .ApplyIncludes(includes)
                    .OrderBy(filter.OrderBy)
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        public List<PersonOrganizationListDataItem> GetPersonsForOrganizationList(QPersonFilter filter)
        {
            using (var db = CreateContext())
            {
                return CreateQuery(filter, db)
                    .OrderBy(filter.OrderBy)
                    .Select(x => new PersonOrganizationListDataItem
                    {
                        OrganizationIdentifier = x.Organization.OrganizationIdentifier,
                        OrganizationName = x.Organization.CompanyName,
                        OrganizationCode = x.Organization.OrganizationCode,
                        OrganizationDomain = x.Organization.CompanyDomain,
                        OrganizationStatus = x.Organization.AccountStatus,
                        OrganizationUrl = x.Organization.CompanyWebSiteUrl,

                        PersonIsAdministrator = x.IsAdministrator,
                        PersonIsDeveloper = x.IsDeveloper,
                        PersonIsLearner = x.IsLearner,
                        PersonIsOperator = x.IsOperator,

                        PersonIsGrantedAccess = x.UserAccessGranted != null
                    })
                    .Distinct()
                    .ApplyPaging(filter.Paging)
                    .ToList();
            }
        }

        private IQueryable<QPerson> CreateQuery(QPersonFilter filter, InternalDbContext db)
        {
            var query = db.QPersons.AsNoTracking().AsQueryable();

            if (filter.OrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationIdentifier.Value);

            if (filter.OrganizationOrParentOrganizationIdentifier.HasValue)
                query = query.Where(x => x.OrganizationIdentifier == filter.OrganizationOrParentOrganizationIdentifier.Value || x.Organization.ParentOrganizationIdentifier == filter.OrganizationOrParentOrganizationIdentifier.Value);

            if (filter.UserIdentifiers.IsNotEmpty())
            {
                if (filter.UserIdentifiers.Length == 1)
                {
                    var value = filter.UserIdentifiers[0];
                    query = query.Where(x => x.UserIdentifier == value);
                }
                else
                    query = query.Where(x => filter.UserIdentifiers.Contains(x.UserIdentifier));
            }

            if (filter.ExcludeUserIdentifiers.IsNotEmpty())
            {
                if (filter.ExcludeUserIdentifiers.Length == 1)
                {
                    var value = filter.ExcludeUserIdentifiers[0];
                    query = query.Where(x => x.UserIdentifier != value);
                }
                else
                    query = query.Where(x => !filter.ExcludeUserIdentifiers.Contains(x.UserIdentifier));
            }

            if (filter.EmployerGroupIdentifier.HasValue)
                query = query.Where(x => x.EmployerGroupIdentifier == filter.EmployerGroupIdentifier.Value);

            if (filter.UserMembershipGroupIdentifier.HasValue)
                query = query.Where(x => x.User.Memberships.Any(y => y.GroupIdentifier == filter.UserMembershipGroupIdentifier.Value));

            if (filter.UserMembershipGroupTypeContains.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => y.Group.GroupType.Contains(filter.UserMembershipGroupTypeContains)));

            if (filter.UserMembershipGroupTypeExact.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => y.Group.GroupType == filter.UserMembershipGroupTypeExact));

            if (filter.UserMembershipGroupLabelContains.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => y.Group.GroupLabel.Contains(filter.UserMembershipGroupLabelContains)));

            if (filter.UserMembershipGroupLabelExact.IsNotEmpty())
                query = query.Where(x => x.User.Memberships.Any(y => y.Group.GroupLabel == filter.UserMembershipGroupLabelExact));

            if (filter.UserNameContains.IsNotEmpty())
                query = query.Where(
                    x => x.User.FullName.Contains(filter.UserNameContains)
                      || x.User.FirstName.Contains(filter.UserNameContains)
                      || x.User.LastName.Contains(filter.UserNameContains)
                      || x.User.FirstName + " " + x.User.LastName == filter.UserNameContains);

            if (filter.UserNameExact.IsNotEmpty())
                query = query.Where(
                    x => x.User.FullName == filter.UserNameExact
                      || x.User.FirstName == filter.UserNameExact
                      || x.User.LastName == filter.UserNameExact
                      || x.User.FirstName + " " + x.User.LastName == filter.UserNameExact);

            if (filter.UserEmailContains.IsNotEmpty())
                query = query.Where(
                    x => x.User.Email.Contains(filter.UserEmailContains)
                      || x.User.EmailAlternate.Contains(filter.UserEmailContains));

            if (filter.UserEmailExact.IsNotEmpty())
                query = query.Where(
                    x => x.User.Email == filter.UserEmailExact
                      || x.User.EmailAlternate == filter.UserEmailExact);

            if (filter.IsNeedReview)
                query = query.Where(x => x.AccountReviewQueued != null && x.AccountReviewCompleted == null || x.UserAccessGranted == null && x.AccessRevoked == null);

            if (filter.ExcludeUserIdentifiers.IsNotEmpty())
            {
                if (filter.ExcludeUserIdentifiers.Length == 1)
                {
                    var value = filter.ExcludeUserIdentifiers[0];
                    query = query.Where(x => x.UserIdentifier != value);
                }
                else
                    query = query.Where(x => !filter.ExcludeUserIdentifiers.Contains(x.UserIdentifier));
            }

            if (filter.PersonCodes.IsNotEmpty())
            {
                if (filter.PersonCodes.Length == 1)
                {
                    var value = filter.PersonCodes[0];
                    query = query.Where(x => x.PersonCode == value);
                }
                else
                    query = query.Where(x => filter.PersonCodes.Contains(x.PersonCode));
            }

            if (filter.IsAdministrator.HasValue)
                query = query.Where(x => x.IsAdministrator == filter.IsAdministrator.Value);

            if (filter.UserNameOrPersonCodeContains.IsNotEmpty())
                query = query.Where(
                    x => x.User.FullName.Contains(filter.UserNameOrPersonCodeContains)
                      || x.PersonCode.Contains(filter.UserNameOrPersonCodeContains));

            if (filter.HasPersonCode.HasValue)
            {
                if (filter.HasPersonCode.Value)
                    query = query.Where(x => x.PersonCode != null);
                else
                    query = query.Where(x => x.PersonCode == null);
            }

            return query;
        }

        private static InternalDbContext CreateContext()
        {
            var db = new InternalDbContext();
            db.Configuration.LazyLoadingEnabled = false;
            return db;
        }
    }
}
