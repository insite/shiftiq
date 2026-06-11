using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using InSite.Domain.Contacts;

using Shift.Sdk.UI;

namespace InSite.Application.Contacts.Read
{
    public interface IPersonSearch
    {
        bool IsPersonExist(Guid userId, Guid organizationId);
        bool IsPersonExist(Guid organizationId, string personCode, Guid? exceptUserId = null);
        bool IsPersonExist(QPersonFilter filter);
        QPerson GetPerson(Guid personId, params Expression<Func<QPerson, object>>[] includes);
        QPerson GetPerson(Guid userId, Guid organizationId, params Expression<Func<QPerson, object>>[] includes);
        QPerson GetPerson(Guid organizationId, string email, params Expression<Func<QPerson, object>>[] includes);
        QPerson GetPerson(string personCode, Guid organizationId, params Expression<Func<QPerson, object>>[] includes);
        List<QPerson> GetPersonsByEmployer(Guid employerGroupId);
        List<QPerson> GetPersonsByEmails(IEnumerable<string> emails, Guid organizationId, params Expression<Func<QPerson, object>>[] includes);
        List<QPerson> GetPersonsByAlternateEmails(IEnumerable<string> alternateEmails, Guid organizationId, params Expression<Func<QPerson, object>>[] includes);
        List<QPerson> GetPersonsByPersonCodes(IEnumerable<string> personCodes, Guid organizationId, params Expression<Func<QPerson, object>>[] includes);
        QPersonAddress GetPersonAddress(Guid addressId);
        List<VDevPerson> GetDevPersons();
        List<string> GetPersonCodes(Guid organizationId, string[] codes = null);
        int CountPersons(QPersonFilter filter);
        List<QPerson> GetPersons(QPersonFilter filter, params Expression<Func<QPerson, object>>[] includes);
        List<PersonOrganizationListDataItem> GetPersonsForOrganizationList(QPersonFilter filter);

        List<string> GetJobDivisions(Guid organizationId);
        List<PersonName> GetNames(Guid organizationId);
    }
}
