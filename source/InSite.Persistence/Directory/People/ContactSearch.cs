using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class ContactSearch : IContactSearch
    {
        private TResult ExecuteWithNoLock<TResult>(Func<InternalDbContext, TResult> search)
        {
            TResult result;

            using (var db = new InternalDbContext(false))
            {
                using (var transaction = db.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    result = search(db);
                    transaction.Commit();
                }
            }

            return result;
        }

        public VAddress GetAddress(Guid addressIdentifier)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VAddresses.FirstOrDefault(x => x.AddressIdentifier == addressIdentifier);
            });
        }

        public VGroup GetGroup(Guid group)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VGroups.FirstOrDefault(x => x.GroupIdentifier == group);
            });
        }

        public VPerson GetPerson(string email, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.FirstOrDefault(x => x.UserEmail == email && x.OrganizationIdentifier == organization);
            });
        }

        public VPerson GetPerson(Guid user, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.FirstOrDefault(x => x.UserIdentifier == user && x.OrganizationIdentifier == organization);
            });
        }

        public string GetPersonLanguage(Guid user, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.Where(x => x.UserIdentifier == user && x.OrganizationIdentifier == organization).Select(x => x.Language).FirstOrDefault();
            });
        }

        public EmailAddress GetEmailAddress(Guid user, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                var data = db.VPersons
                    .Where(p => p.UserIdentifier == user && p.OrganizationIdentifier == organization)
                    .Select(p => new
                    {
                        p.UserIdentifier,
                        p.UserFullName,
                        p.UserEmail,
                        p.UserEmailEnabled,
                        p.UserEmailAlternate,
                        p.UserEmailAlternateEnabled,
                        p.Language,
                        p.PersonCode
                    })
                    .FirstOrDefault();

                if (data == null)
                    return null;

                var email = EmailAddress.GetEnabledEmail(data.UserEmail, data.UserEmailEnabled, data.UserEmailAlternate, data.UserEmailAlternateEnabled);
                if (email.IsEmpty())
                    return null;

                return new EmailAddress(data.UserIdentifier, email, data.UserFullName, data.PersonCode, data.Language);
            });
        }

        public EmailAddress[] GetEmailAddresses(IEnumerable<Guid> users, Guid organization)
        {
            if (users == null || !users.Any())
                return new EmailAddress[0];

            return ExecuteWithNoLock((db) =>
            {
                var data = db.VPersons
                    .Where(p => users.Contains(p.UserIdentifier) && p.OrganizationIdentifier == organization)
                    .Select(p => new
                    {
                        p.UserIdentifier,
                        p.UserFullName,
                        p.UserEmail,
                        p.UserEmailEnabled,
                        p.UserEmailAlternate,
                        p.UserEmailAlternateEnabled,
                        p.Language,
                        p.PersonCode
                    })
                    .ToArray();
                var result = new List<EmailAddress>();

                foreach (var item in data)
                {
                    var email = EmailAddress.GetEnabledEmail(item.UserEmail, item.UserEmailEnabled, item.UserEmailAlternate, item.UserEmailAlternateEnabled);
                    if (!email.IsEmpty())
                        result.Add(new EmailAddress(item.UserIdentifier, email, item.UserFullName, item.PersonCode, item.Language));
                }

                return result.ToArray();
            });
        }

        public VPerson[] GetPersons(IEnumerable<Guid> users, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.Where(x => users.Contains(x.UserIdentifier) && x.OrganizationIdentifier == organization).ToArray();
            });
        }

        public VPerson GetPersonByCode(string code, Guid organization)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.FirstOrDefault(x => x.PersonCode == code && x.OrganizationIdentifier == organization);
            });
        }

        public VUser GetUser(Guid user)
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VUsers.Where(x => x.UserIdentifier == user).FirstOrDefault();
            });
        }

        public List<VPerson> GetPeopleWithEmailEnabled()
        {
            return ExecuteWithNoLock((db) =>
            {
                return db.VPersons.Where(x => x.UserEmailEnabled).ToList();
            });
        }

        public List<DateTimeOffset> GetHolidays()
        {
            return new List<DateTimeOffset>();
        }

        #region Bind

        #region Public

        public T[] Bind<T>(
            Expression<Func<VUser, T>> binder,
            QUserFilter filter) =>
            UserReadHelper.Instance.Bind(binder, filter);

        public T[] Bind<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            UserReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public T[] Bind<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null) =>
            UserReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);

        public T BindFirst<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            UserReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        public int Count(QUserFilter filter)
        {
            using (var db = new InternalDbContext(false))
                return db.VUsers.AsQueryable().Filter(filter).Count();
        }

        public T[] BindPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            PersonReadHelper.Instance.Bind(binder, filter, modelSort, entitySort);

        public T[] BindPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null) =>
            PersonReadHelper.Instance.Bind(binder, filter, paging, modelSort, entitySort);

        public T BindFirstPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            string modelSort = null,
            string entitySort = null) =>
            PersonReadHelper.Instance.BindFirst(binder, filter, modelSort, entitySort);

        #endregion

        #region Private

        private class UserReadHelper : ReadHelper<VUser>
        {
            public static readonly UserReadHelper Instance = new UserReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VUser>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.VUsers.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }

            public T[] Bind<T>(Expression<Func<VUser, T>> binder, QUserFilter filter)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.VUsers.AsQueryable().AsNoTracking();

                    var modelQuery = BuildQuery(
                        query,
                        (IQueryable<VUser> q) => q.Select(binder),
                        (IQueryable<VUser> q) => q.Filter(filter),
                        q => q,
                        filter.Paging,
                        filter.OrderBy,
                        null,
                        false
                    );

                    return modelQuery.ToArray();
                }
            }
        }

        private class PersonReadHelper : ReadHelper<VPerson>
        {
            public static readonly PersonReadHelper Instance = new PersonReadHelper();

            protected override TResult ExecuteQuery<TResult>(Func<IQueryable<VPerson>, TResult> func)
            {
                using (var context = new InternalDbContext(false))
                {
                    var query = context.VPersons.AsQueryable().AsNoTracking();

                    return func(query);
                }
            }
        }

        #endregion

        #endregion
    }
}
