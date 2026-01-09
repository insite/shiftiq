using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Contacts.Read;

namespace InSite.Common.Web.UI
{
    public class FindPersonCodeUser : BaseFindEntity<QPersonFilter>
    {
        #region Properties

        public QPersonFilter Filter => (QPersonFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QPersonFilter()));

        #endregion

        protected override string GetEntityName() => "Person";

        protected override QPersonFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.HasPersonCode = true;
            filter.UserNameOrPersonCodeContains = keyword;
            filter.OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier;

            return filter;
        }

        protected override int Count(QPersonFilter filter)
        {
            return ServiceLocator.PersonSearch.CountPersons(filter);
        }

        protected override DataItem[] Select(QPersonFilter filter)
        {
            filter.OrderBy = nameof(QPerson.PersonCode);

            return ServiceLocator.PersonSearch
                .GetPersons(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.PersonSearch
                .GetPersons(new QPersonFilter
                {
                    UserIdentifiers = ids,
                    OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
                })
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(QPerson x) => new DataItem
        {
            Value = x.UserIdentifier,
            Text = x.PersonCode
        };
    }
}