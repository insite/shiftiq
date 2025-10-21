using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class FindBankOccupation : BaseFindEntity<FindBankOccupation.DataFilter>
    {
        #region Classes

        public class DataFilter : Filter
        {
            public Guid OrganizationIdentifier { get; set; }
            public string Keyword { get; set; }
        }

        #endregion

        protected override string GetEntityName() => "Bank Occupation";

        protected override DataFilter GetFilter(string keyword)
        {
            return new DataFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier,
                Keyword = keyword
            };
        }

        protected override int Count(DataFilter filter)
        {
            return ServiceLocator.BankSearch.CountBankOccupations(filter.OrganizationIdentifier, filter.Keyword);
        }

        protected override DataItem[] Select(DataFilter filter)
        {
            return ServiceLocator.BankSearch
                .GetBankOccupations(
                    filter.OrganizationIdentifier,
                    filter.Paging,
                    filter.Keyword)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            var filter = GetFilter((string)null);

            return ServiceLocator.BankSearch
                .GetBankOccupations(filter.OrganizationIdentifier, ids)
                .Select(GetDataItem);
        }

        private static DataItem GetDataItem(BankSummaryOccupationInfo x) => new DataItem
        {
            Value = x.OccupationID,
            Text = x.OccupationTitle
        };
    }
}