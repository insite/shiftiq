using System;
using System.Linq;
using System.Web.UI;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common.Events;

namespace InSite.Cmds.Controls.Contacts.Categories
{
    public partial class CategoryGrid : UserControl
    {
        public event IntValueHandler Refreshed;
        private void OnRefreshed(int count)
            => Refreshed?.Invoke(this, new IntValueArgs(count));

        private Guid OrganizationIdentifier
        {
            get { return (Guid)ViewState[nameof(OrganizationIdentifier)]; }
            set { ViewState[nameof(OrganizationIdentifier)] = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.EnablePaging = false;
            Grid.DataBinding += Grid_DataBinding;
            Grid.RowCommand += Grid_RowCommand;
        }

        public int LoadData(Guid organization)
        {
            OrganizationIdentifier = organization;

            Grid.DataBind();

            CreatorLink.NavigateUrl = string.Format("/ui/cmds/admin/organizations/categories/create?organizationID={0}", organization);

            return Grid.Rows.Count;
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            Grid.DataSource = VAchievementCategorySearch
                .Select(x => x.OrganizationIdentifier == OrganizationIdentifier)
                .OrderBy(x => x.AchievementLabel)
                .ThenBy(x => x.CategoryName);
        }

        private void Grid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var grid = (Grid)sender;
                var categoryIdentifier = grid.GetDataKey<Guid>(e);

                TAchievementCategoryStore.Delete(categoryIdentifier);

                Grid.DataBind();

                OnRefreshed(Grid.Rows.Count);
            }
        }
    }
}