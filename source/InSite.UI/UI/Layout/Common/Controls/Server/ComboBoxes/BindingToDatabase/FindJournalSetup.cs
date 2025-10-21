using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

namespace InSite.Common.Web.UI
{
    public class FindJournalSetup : BaseFindEntity<QJournalSetupFilter>
    {
        #region Properties

        public QJournalSetupFilter Filter => (QJournalSetupFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QJournalSetupFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier }));

        #endregion

        protected override string GetEntityName() => "Journal Setup";

        protected override QJournalSetupFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.JournalSetupName = keyword;

            return filter;
        }

        protected override int Count(QJournalSetupFilter filter)
        {
            return ServiceLocator.JournalSearch.CountJournalSetups(filter);
        }

        protected override DataItem[] Select(QJournalSetupFilter filter)
        {
            filter.OrderBy = nameof(QJournalSetup.JournalSetupName);

            return ServiceLocator.JournalSearch
                .GetJournalSetups(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.JournalSearch
                .GetJournalSetups(ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(QJournalSetup x) => new DataItem
        {
            Value = x.JournalSetupIdentifier,
            Text = x.JournalSetupName,
        };
    }
}