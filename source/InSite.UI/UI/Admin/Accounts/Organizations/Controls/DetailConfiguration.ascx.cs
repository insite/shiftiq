using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;

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

            DownloadPermissionMatrix.Click += DownloadPermissionMatrix_Click;
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
                if (selectedTab == "security")
                    SecurityTab.IsSelected = true;

                if (selectedTab == "url")
                    UrlTab.IsSelected = true;
            }

            BindSecurityModelToControls(organization);
        }

        private void BindSecurityModelToControls(OrganizationState organization)
        {
            var resourcePermissions = TActionSearch.SearchPermissionsGroupedByResource(organization.Code);

            var json = JsonConvert.SerializeObject(resourcePermissions, CreateJsonSerializerSettings());

            AccessGrantedToActions.Text = json;

            var organizationPermission = TGroupPermissionStore.GetAccess(organization.Identifier);

            AccessGranted.Text = organizationPermission.AccessGranted;

            AccessDenied.Text = organizationPermission.AccessDenied;
        }

        private JsonSerializerSettings CreateJsonSerializerSettings()
            => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                ContractResolver = new SkipEmptyCollectionsContractResolver(),
                Converters = new List<JsonConverter>
                    {
                        new FlagsEnumConverter<FeatureAccess>(),
                        new FlagsEnumConverter<DataAccess>(),
                        new FlagsEnumConverter<AuthorityAccess>()
                    }
            };

        public void GetInputValues(OrganizationState organization)
        {
            DetailPlatform.GetInputValues(organization);
            DetailLocationDescription.GetInputValues(organization);
            DetailLocationAddress.GetInputValues(organization);
            DetailCustomizationUrl.GetInputValues(organization);
            DetailBambora.GetInputValues(organization);
            DetailUpload.GetInputValues(organization);

            Snapshot.Text = JsonConvert.SerializeObject(organization);

            TGroupPermissionStore.SaveAccess(
                organization.Identifier,
                AccessGrantedToActions.Text,
                AccessGranted.Text,
                AccessDenied.Text);
        }

        private void DownloadPermissionMatrix_Click(object sender, EventArgs e)
        {
            var organization = OrganizationSearch.Select(OrganizationId.Value);

            var permissions = PermissionCache.Matrix.GetPermissions(organization.Code);

            var json = JsonConvert.SerializeObject(permissions, _settings);

            var data = Encoding.UTF8.GetBytes(json);

            HttpResponseHelper.SendFile(Response, $"permission-matrix-{organization.Code}.json", data);
        }

        private JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new SkipEmptyCollectionsContractResolver(),
            Converters = new List<JsonConverter>
            {
                new FlagsEnumConverter<FeatureAccess>(),
                new FlagsEnumConverter<DataAccess>(),
                new FlagsEnumConverter<AuthorityAccess>()
            }
        };
    }
}