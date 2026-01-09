using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;

using AccountStatusEnum = Shift.Constant.AccountStatus;

namespace InSite.UI.Admin.Accounts.Organizations.Controls
{
    public partial class DetailOrganization : BaseUserControl
    {
        #region Events

        public event EventHandler Updated;
        private void OnUpdated() => Updated?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Properties

        protected Guid? OrganizationId
        {
            get => (Guid?)ViewState[nameof(OrganizationId)];
            set => ViewState[nameof(OrganizationId)] = value;
        }

        #endregion

        #region Intialization

        protected override void OnInit(EventArgs e)
        {
            DomainPatternValidator.ValidationExpression = Pattern.ValidDomain;

            OrganizationCodeValidator.ServerValidate += OrganizationCodeValidator_ServerValidate;

            ReopenOrganizationButton.Click += ReopenOrganizationButton_Click;
            CloseOrganizationLink.Click += CloseOrganizationLink_Click;

            base.OnInit(e);
        }

        #endregion

        #region Event handlers

        private void OrganizationCodeValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var organization = OrganizationSearch.Select(args.Value);

            args.IsValid = organization == null || organization.OrganizationIdentifier == OrganizationId;

            if (!args.IsValid)
                OrganizationCodeValidator.ErrorMessage = $@"The account code <strong>{args.Value}</strong> is already assigned to another organization.";
        }

        private void ReopenOrganizationButton_Click(object sender, EventArgs e)
        {
            OrganizationStore.Open(OrganizationId.Value);

            OnUpdated();
        }

        private void CloseOrganizationLink_Click(object sender, EventArgs e)
        {
            OrganizationStore.Close(OrganizationId.Value);

            OnUpdated();
        }

        #endregion

        #region Setting and getting input values

        public void SetDefaultValues()
        {
            SetupContainer.Visible = false;

            TimeZone.Value = TimeZones.Pacific.Id;

            Languages.Values = new[] { "en" };
        }

        public void SetInputValues(OrganizationState organization)
        {
            OrganizationId = organization.OrganizationIdentifier;

            ParentOrganizationIdentifier.Value = organization.ParentOrganizationIdentifier;

            var hasParent = organization.ParentOrganizationIdentifier.HasValue;

            if (hasParent)
            {
                var parent = OrganizationSearch.Select(organization.ParentOrganizationIdentifier.Value);

                hasParent = parent != null;

                if (hasParent)
                    ParentLink.NavigateUrl = $"/ui/admin/accounts/organizations/edit?organization={parent.OrganizationIdentifier}";
            }

            ParentLink.Visible = hasParent;

            OrganizationCode.Text = organization.OrganizationCode;
            CompanyName.Text = organization.CompanyName;
            CompanyDomain.Text = organization.CompanyDomain;

            if (organization.AccountClosed.HasValue)
            {
                AccountStatus.Text = @"<span class='badge bg-danger'><i class='far fa-folder'></i> Closed</span>";
                AccountStatusHelp.Text = $@"This account was closed {organization.AccountClosed.Humanize()}, on {organization.AccountClosed:MMMM d, yyyy}.";
            }
            else
            {
                AccountStatus.Text = @"<span class='badge bg-success'><i class='far fa-folder-open'></i> Open</span>";
                AccountStatusHelp.Text = $@"This account was opened {organization.AccountOpened.Humanize()}, on {organization.AccountOpened:MMMM d, yyyy}.";
            }

            ReopenOrganizationButton.Visible = organization.AccountClosed.HasValue;
            CloseOrganizationLink.Visible = organization.AccountClosed == null;

            OrganizationAnnouncement.Text = organization.AccountWarning;

            CompanyAdministratorIdentifier.Filter.OrganizationIdentifier = organization.OrganizationIdentifier;
            CompanyAdministratorIdentifier.Value = organization.AdministratorUserIdentifier;

            var sizeOption = CompanySize.FindOptionByValue(organization.CompanyDescription.CompanySize.ToString(), true);
            if (sizeOption != null)
                sizeOption.Selected = true;

            var webSiteUrl = organization.PlatformCustomization.TenantUrl.WebSite;
            if (webSiteUrl.IsNotEmpty() && !webSiteUrl.Contains("://"))
                webSiteUrl = "https://" + webSiteUrl;

            CompanyTitle.Text = organization.CompanyDescription.LegalName;
            Description.Text = organization.CompanyDescription.CompanySummary;
            CompanyWebSiteUrl.Text = webSiteUrl;

            Languages.Values = organization.Languages.Select(x => x.TwoLetterISOLanguageName).ToArray();
            TimeZone.Value = organization.TimeZone.Id;
        }

        public void GetInputValues(OrganizationState organization)
        {
            organization.ParentOrganizationIdentifier = ParentOrganizationIdentifier.Value ?? Guid.Empty;
            organization.OrganizationCode = OrganizationCode.Text?.ToLower();
            organization.CompanyName = CompanyName.Text;
            organization.CompanyDomain = CompanyDomain.Text;
            organization.AccountWarning = OrganizationAnnouncement.Text;

            organization.AdministratorUserIdentifier = CompanyAdministratorIdentifier.Value;
            organization.CompanyDescription.CompanySize = CompanySize.Value.ToEnum<CompanySize>();
            organization.CompanyDescription.LegalName = CompanyTitle.Text;
            organization.CompanyDescription.CompanySummary = Description.Text;
            organization.PlatformCustomization.TenantUrl.WebSite = CompanyWebSiteUrl.Text;

            {
                var languages = new List<CultureInfo> { new CultureInfo("en") };
                foreach (var selection in Languages.Values)
                    languages.Add(new CultureInfo(selection));
                organization.Languages = languages.ToArray();
            }
            organization.TimeZone = TimeZones.GetInfo(TimeZone.Value);
        }

        #endregion
    }
}