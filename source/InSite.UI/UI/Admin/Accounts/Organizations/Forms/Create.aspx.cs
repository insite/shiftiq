using System;

using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Accounts.Organizations.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/admin/accounts/organizations/edit";
        private const string SearchUrl = "/ui/admin/accounts/organizations/search";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(Page);

            DetailOrganization.SetDefaultValues();
            DetailLocationAddress.EnableValidators(true);

            CancelButton.NavigateUrl = SearchUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var organization = new OrganizationState
            {
                OrganizationIdentifier = UniqueIdentifier.Create(),
                ParentOrganizationIdentifier = Guid.Empty,
                AccountOpened = DateTimeOffset.UtcNow,
                Toolkits = new ToolkitSettings
                {
                    Contacts = new ContactSettings
                    {
                        FullNamePolicy = "{First} {Middle} {Last}"
                    }
                }
            };

            DetailOrganization.GetInputValues(organization);
            DetailLocationAddress.GetInputValues(organization);

            OrganizationStore.Insert(organization);

            PersonStore.Insert(PersonFactory.Create(User.UserIdentifier, organization.OrganizationIdentifier, null, false, null));

            OrganizationSearch.Refresh();

            HttpResponseHelper.Redirect($"{EditUrl}?organization={organization.OrganizationIdentifier}&status=new");
        }
    }
}