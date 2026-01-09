using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GroupLabelComboBox : ComboBox
    {
        public string GroupType
        {
            get => (string)ViewState[nameof(GroupType)];
            set => ViewState[nameof(GroupType)] = value;
        }

        protected override ListItemArray CreateDataSource()
        {
            var organization = CurrentSessionState.Identity.Organization.Identifier;
            var labels = ServiceLocator.GroupSearch.GetGroupLabels(organization, GroupType);

            var list = new ListItemArray();
            foreach (var label in labels)
                list.Add(label);

            return list;
        }
    }
}