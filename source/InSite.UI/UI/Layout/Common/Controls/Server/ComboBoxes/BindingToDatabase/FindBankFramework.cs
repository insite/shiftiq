using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindBankFramework : BaseFindEntity<FindBankFramework.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public Guid OrganizationIdentifier { get; set; }
            public Guid? OccupationIdentifier { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        #region Properties

        public Guid? OccupationID
        {
            get => (Guid?)ViewState[nameof(OccupationID)];
            set => ViewState[nameof(OccupationID)] = value;
        }

        #endregion

        protected override string GetEntityName() => "Bank Framework";

        protected override DataFilter GetFilter(string keyword) => new DataFilter
        {
            OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
            OccupationIdentifier = OccupationID,
            Keyword = keyword
        };

        protected override int Count(DataFilter filter)
        {
            return ServiceLocator.BankSearch.CountBankFrameworks(
                filter.OrganizationIdentifier,
                filter.OccupationIdentifier,
                filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            return ServiceLocator.BankSearch
                .GetBankFrameworks(
                    filter.OrganizationIdentifier,
                    filter.OccupationIdentifier,
                    filter.Paging,
                    filter.Keyword)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = GetFilter((string)null);

            return ServiceLocator.BankSearch
                .GetBankFrameworks(filter.OrganizationIdentifier, ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(BankSummaryFrameworkInfo x) => new DataItem
        {
            Value = x.FrameworkID,
            Text = x.FrameworkTitle
        };
    }
}