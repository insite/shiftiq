using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GroupMultiComboBox : MultiComboBox
    {
        public QGroupSelectorFilter ListFilter => (QGroupSelectorFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QGroupSelectorFilter()));

        protected override ListItemArray CreateDataSource()
        {
            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(ListFilter, false);

            var data =  groups.Select(
                    x => new ListItem
                    {
                        Value = x.GroupIdentifier.ToString(),
                        Text = x.GroupName
                    }).ToList();

            return new ListItemArray(data);
        }
    }
}