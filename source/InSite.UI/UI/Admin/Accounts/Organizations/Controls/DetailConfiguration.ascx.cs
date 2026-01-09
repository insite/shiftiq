using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Newtonsoft.Json;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailConfiguration : BaseUserControl
    {
        protected Guid? OrganizationId
        {
            get => (Guid?)ViewState[nameof(OrganizationId)];
            set => ViewState[nameof(OrganizationId)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            Snapshot.AutoPostBack = true;
            Snapshot.TextChanged += Snapshot_TextChanged;

            SnapshotValidator.ServerValidate += SnapshotValidator_ServerValidate;

            base.OnInit(e);
        }

        private void Snapshot_TextChanged(object sender, EventArgs e)
        {
            Page.Validate();

            if (!OrganizationId.HasValue || !Page.IsValid)
                return;

            OrganizationStore.Update(JsonConvert.DeserializeObject<OrganizationState>(Snapshot.Text));
            HttpResponseHelper.Redirect($"/ui/admin/accounts/organizations/edit?organization={OrganizationId.Value}");
        }

        private void SnapshotValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                JsonConvert.DeserializeObject<OrganizationState>(Snapshot.Text);
            }
            catch
            {
                args.IsValid = false;
            }
        }

        public void SetInputValues(OrganizationState organization, string selectedPanel, string selectedTab)
        {
            OrganizationId = organization.OrganizationIdentifier;

            Snapshot.Text = JsonConvert.SerializeObject(organization, Formatting.Indented);

            DetailPlatform.SetInputValues(organization);
            DetailLocationDescription.SetInputValues(organization);
            DetailLocationAddress.SetInputValues(organization);
            DetailCustomizationUrl.SetInputValues(organization);
            DetailBambora.SetInputValues(organization);
            DetailUpload.SetInputValues(organization);

            if (selectedPanel == "configuration")
            {
                if (selectedTab == "url")
                    UrlTab.IsSelected = true;
            }
        }

        public void GetInputValues(OrganizationState organization)
        {
            DetailPlatform.GetInputValues(organization);
            DetailLocationDescription.GetInputValues(organization);
            DetailLocationAddress.GetInputValues(organization);
            DetailCustomizationUrl.GetInputValues(organization);
            DetailBambora.GetInputValues(organization);
            DetailUpload.GetInputValues(organization);

            Snapshot.Text = JsonConvert.SerializeObject(organization);
        }
    }
}