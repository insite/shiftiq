using System;

using InSite.Application.Banks.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class BankFormComboBox : ComboBox
    {
        #region Properties

        public bool DisablePublished
        {
            get => (bool)(ViewState[nameof(DisablePublished)] ?? false);
            set => ViewState[nameof(DisablePublished)] = value;
        }

        public QBankFormFilter ListFilter => (QBankFormFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QBankFormFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        #endregion

        #region Select data

        protected override ListItemArray CreateDataSource()
        {
            var filter = ListFilter.Clone();

            filter.OrderBy = $"{nameof(QBankForm.FormName)},{nameof(QBankForm.FormTitle)}";

            var list = new ListItemArray();

            foreach (var dataItem in ServiceLocator.BankSearch.GetForms(filter))
            {
                var listItem = list.Add(dataItem.FormIdentifier.ToString(), GetText(dataItem));
                if (DisablePublished && string.Equals(dataItem.FormPublicationStatus, "Published", StringComparison.OrdinalIgnoreCase))
                    listItem.Enabled = false;
            }

            return list;
        }

        #endregion

        #region Helper methods

        private static string GetText(QBankForm entity) =>
            entity.FormName ?? entity.FormTitle;

        #endregion
    }
}