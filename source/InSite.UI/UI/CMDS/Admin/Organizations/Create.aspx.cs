using System;
using System.Globalization;

using InSite.Common.Web;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Admin.Organizations.Forms
{
    public partial class Create : AdminBasePage
    {
        private const string EditUrl = "/ui/cmds/admin/organizations/edit";

        private const string SearchUrl = "/ui/cmds/admin/organizations/search";

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                PageHelper.AutoBindHeader(this);

                CancelButton.NavigateUrl = SearchUrl;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (IsValid)
                Save();
        }

        private void Save()
        {
            var parent = OrganizationSearch.Select("cmds");
            var organization = new OrganizationState
            {
                ParentOrganizationIdentifier = parent.OrganizationIdentifier,
                OrganizationIdentifier = UniqueIdentifier.Create(),
                AccountStatus = AccountStatus.Opened,
                AccountOpened = DateTimeOffset.UtcNow,
                AccountClosed = null,
                CompanyName = null,
                CompanyDomain = null,
                CompanyDescription =
                {
                    CompanySummary = null,
                    LegalName = null
                },
                TimeZone = TimeZones.Mountain,
                PlatformCustomization = new PlatformCustomization(),
                Languages = new CultureInfo[] { new CultureInfo("en") },
            };

            GetInputValues(organization);

            organization.OrganizationCode = OrganizationSearch.CreateNewOrganizationCode(organization.OrganizationCode);

            OrganizationStore.Insert(organization);

            PersonStore.Insert(PersonFactory.Create(User.UserIdentifier, organization.OrganizationIdentifier, null, false, null));

            HttpResponseHelper.Redirect($"{EditUrl}?id={organization.OrganizationIdentifier}&status=saved");
        }

        public void GetInputValues(OrganizationState organization)
        {
            organization.CompanyDescription.LegalName = CompanyName.Text;
            organization.CompanyName = Acronym.Text;
            organization.OrganizationCode = OrganizationCode.Text;
            organization.CompanyDescription.CompanySummary = Description.Text;
            organization.CompanyDomain = WebSiteUrl.Text;
        }
    }
}