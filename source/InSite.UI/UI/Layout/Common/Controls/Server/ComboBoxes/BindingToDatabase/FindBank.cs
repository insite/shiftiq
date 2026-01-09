using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;

namespace InSite.Common.Web.UI
{
    public class FindBank : BaseFindEntity<QBankFilter>
    {
        #region Properties

        public QBankFilter Filter => (QBankFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QBankFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Bank";
        
        protected override QBankFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(QBankFilter filter)
        {
            return ServiceLocator.BankSearch.CountBanks(filter);
        }

        protected override DataItem[] Select(QBankFilter filter)
        {
            filter.OrderBy = $"{nameof(QBank.BankName)},{nameof(QBank.BankTitle)}";

            return ServiceLocator.BankSearch
                .GetBanks(filter)
                .Select(GetDataItem)
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return ServiceLocator.BankSearch
                .GetBanks(ids)
                .Select(GetDataItem)
                .ToArray();
        }

        private static DataItem GetDataItem(QBank x) => new DataItem
        {
            Value = x.BankIdentifier,
            Text = x.BankName ?? x.BankTitle,
        };
    }
}