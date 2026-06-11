using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Shift.Common;

namespace InSite.Application.Contacts.Read
{
    public interface IContactSearch
    {
        VAddress GetAddress(Guid address);

        VGroup GetGroup(Guid group);

        VPerson GetPerson(Guid user, Guid organization);
        VPerson[] GetPersons(IEnumerable<Guid> users, Guid organization);
        VPerson GetPersonByCode(string code, Guid organization);
        string GetPersonLanguage(Guid user, Guid organization);
        EmailAddress GetEmailAddress(Guid user, Guid organization);
        EmailAddress[] GetEmailAddresses(IEnumerable<Guid> users, Guid organization);

        VUser GetUser(Guid user);

        List<DateTimeOffset> GetHolidays();

        T[] Bind<T>(Expression<Func<VUser, T>> binder, QUserFilter filter);

        T[] Bind<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        T[] Bind<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null);

        T BindFirst<T>(
            Expression<Func<VUser, T>> binder,
            Expression<Func<VUser, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        int Count(QUserFilter filter);


        T[] BindPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            string modelSort = null,
            string entitySort = null);

        T[] BindPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            Paging paging,
            string modelSort = null,
            string entitySort = null);

        T BindFirstPerson<T>(
            Expression<Func<VPerson, T>> binder,
            Expression<Func<VPerson, bool>> filter,
            string modelSort = null,
            string entitySort = null);
    }
}