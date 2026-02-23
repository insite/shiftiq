using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.UI.Layout.Common.Controls.Navigation
{
    public partial class AdminMenu : UserControl
    {
        public void LoadData()
        {
            var menuGroups = MenuHelperFactory.Create().GetAdminMenuGroups(new NavigationIdentity(CurrentSessionState.Identity, ServiceLocator.Partition.Slug));

            AdminMenuItem.Visible = menuGroups.Count > 0;

            if (menuGroups.Count == 0)
                return;

            MenuGroups.ItemDataBound += MenuGroups_ItemDataBound;
            MenuGroups.DataSource = menuGroups;
            MenuGroups.DataBind();
        }

        private void MenuGroups_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var menuItems = (Repeater)e.Item.FindControl("MenuItems");
            menuItems.DataSource = ((NavigationList)e.Item.DataItem).MenuItems;
            menuItems.DataBind();
        }
    }
}