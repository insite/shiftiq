using InSite.Application.Banks.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class BankLevelComboBox : ComboBox
    {
        public QBankFilter ListFilter => (QBankFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QBankFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier
            }));

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();
            var data = ServiceLocator.BankSearch.GetBankLevels(ListFilter);

            foreach (var type in data)
                list.Add(new ListItem { Text = type, Value = type });

            return list;
        }
    }
}