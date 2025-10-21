using System.Linq;

using InSite.Application.Contacts.Read;

using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class GroupComboBox : ComboBox
    {
        #region Properties

        public bool ShowCities
        {
            get { return ViewState[nameof(ShowCities)] as bool? ?? false; }
            set { ViewState[nameof(ShowCities)] = value; }
        }

        public ListItem DefaultOptions
        {
            get { return ViewState[nameof(DefaultOptions)] as ListItem; }
            set { ViewState[nameof(DefaultOptions)] = value; }
        }

        public QGroupSelectorFilter ListFilter => (QGroupSelectorFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new QGroupSelectorFilter()));

        #endregion

        #region Data binding

        protected override ListItemArray CreateDataSource()
        {
            ListFilter.AlwaysIncludeGroupIdentifiers = ValueAsGuid.HasValue
                ? new[] { ValueAsGuid.Value }
                : null;

            var groups = ServiceLocator.GroupSearch.GetSelectorGroups(ListFilter, ShowCities);

            var data = ShowCities
                ? groups.Select(
                    x => new ListItem
                    {
                        Value = x.GroupIdentifier.ToString(),
                        Text = x.GroupName + (x.ShippingAddress?.City != null ? " [" + x.ShippingAddress?.City + "]" : "")
                    }).ToList()
                : groups.Select(
                    x => new ListItem
                    {
                        Value = x.GroupIdentifier.ToString(),
                        Text = x.GroupName
                    }).ToList();

            if (DefaultOptions != null)
                data.Add(DefaultOptions);

            return new ListItemArray(data);
        }

        #endregion
    }
}