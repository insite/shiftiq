using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Banks.Read;

namespace InSite.Common.Web.UI
{
    public class FindBankForm : BaseFindEntity<QBankFormFilter>
    {
        #region Properties

        public QBankFormFilter Filter => (QBankFormFilter)(ViewState[nameof(Filter)]
            ?? (ViewState[nameof(Filter)] = new QBankFormFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        protected override string GetEntityName() => "Bank Form";

        protected override QBankFormFilter GetFilter(string keyword)
        {
            var filter = Filter.Clone();

            filter.Keyword = keyword;

            return filter;
        }

        protected override int Count(QBankFormFilter filter)
        {
            return ServiceLocator.BankSearch.CountForms(filter);
        }

        protected override DataItem[] Select(QBankFormFilter filter)
        {
            filter.OrderBy = $"{nameof(QBankForm.FormName)},{nameof(QBankForm.FormTitle)}";

            return ServiceLocator.BankSearch
                .GetForms(filter)
                .Select(x => new DataItem
                {
                    Value = x.FormIdentifier,
                    Text = $"{x.FormName} [{x.FormAsset}.{x.FormAssetVersion}]"
                })
                .ToArray();
        }

        protected override IEnumerable<DataItem> GetItems(Guid[] ids)
        {
            return Select(new QBankFormFilter { FormIdentifiers = ids });
        }
    }
}