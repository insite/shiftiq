using System;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Contract;

namespace InSite.Admin.Accounts.Organizations.Forms
{
    public partial class Edit : AdminBasePage
    {
        private const string SearchUrl = "/ui/admin/accounts/organizations/search";

        private Guid OrganizationID => Guid.TryParse(Request.QueryString["organization"], out var value) ? value : Guid.Empty;
        private string Panel => Request["panel"];
        private string Tab => Request["tab"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Details.Updated += Details_Updated;
            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        private void Details_Updated(object sender, EventArgs e)
        {
            Open();
        }

        private void Open()
        {
            var entity = OrganizationSearch.Select(OrganizationID);

            if (entity == null)
                HttpResponseHelper.Redirect(SearchUrl);

            Details.SetInputValues(entity, Panel, Tab);

            PageHelper.AutoBindHeader(
                Page,
                new BreadcrumbItem("Add New Organization", "/ui/admin/accounts/organizations/create", null, null),
                entity.CompanyName
            );

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var organization = OrganizationSearch.Select(OrganizationID);

            Details.GetInputValues(organization);

            OrganizationStore.Update(organization);

            Details.SaveCollections();

            OrganizationSearch.Refresh();

            Open();

            SetStatus(EditorStatus, StatusType.Saved);
        }
    }
}