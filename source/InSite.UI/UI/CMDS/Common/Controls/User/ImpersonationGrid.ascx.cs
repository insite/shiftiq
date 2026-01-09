using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

using Shift.Common;

namespace InSite.Cmds.Controls.User
{
    public partial class ImpersonationGrid : BaseUserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
        }

        public void LoadData()
        {
            var count = ImpersonationSearch.Count(new ImpersonationFilter());

            Grid.PageIndex = 0;
            Grid.VirtualItemCount = count;
            Grid.DataBind();

            NoDataMessage.Visible = count == 0;
        }

        private void Grid_DataBinding(object source, EventArgs e)
        {
            var filter = new ImpersonationFilter
            {
                Paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize)
            };

            Grid.DataSource = ImpersonationSearch.Select(filter);
        }

        protected string GetImpersonationName(Guid? user, string name)
        {
            string nameTag = string.Format("<strong>{0}</strong>", name);

            return user.HasValue
                ? string.Format("<a href='/ui/cmds/admin/users/edit?userID={1}'>{0}</a>", nameTag, user)
                : nameTag;
        }
    }
}