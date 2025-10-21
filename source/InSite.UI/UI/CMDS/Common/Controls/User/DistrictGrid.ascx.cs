using System;
using System.Web.UI;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;

namespace InSite.Cmds.Controls.Contacts.Companies
{
    public partial class DistrictGrid : UserControl
    {
        private Guid OrganizationIdentifier
        {
            get => (Guid)ViewState[nameof(OrganizationIdentifier)];
            set => ViewState[nameof(OrganizationIdentifier)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Grid.DataBinding += Grid_DataBinding;
        }

        public int LoadData(Guid organizationId)
        {
            OrganizationIdentifier = organizationId;

            Grid.VirtualItemCount = ContactRepository3.CountByDistrictFilter(organizationId);
            Grid.PageIndex = 0;
            Grid.DataBind();

            CreatorLink.NavigateUrl = $"/ui/cmds/admin/divisions/create?organizationID={organizationId}";

            return Grid.VirtualItemCount;
        }

        private void Grid_DataBinding(Object source, EventArgs e)
        {
            var paging = Paging.SetPage(Grid.PageIndex + 1, Grid.PageSize);

            var table = ContactRepository3.SelectByDistrictFilter(OrganizationIdentifier, paging);

            Grid.DataSource = table;
        }
    }
}