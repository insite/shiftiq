using System;

using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.UI.Admin.Reports.Controls
{
    public partial class LoginHistorySearchCriteria : SearchCriteriaController<TUserSessionFilter>
    {
        public override TUserSessionFilter Filter
        {
            get
            {
                var filter = new TUserSessionFilter
                {
                    OrganizationIdentifier = !Identity.IsOperator ? Organization.Identifier : OrganizationIdentifier.Value,
                    UserEmail = UserName.Text,
                    UserHostAddress = UserHostAddress.Text,
                    UserBrowser = UserBrowser.Text,
                    UserLanguage = UserLanguage.Text,
                    SessionIsAuthenticated = IsValid.ValueAsBoolean,
                    SessionStartedSince = SessionStartedSince.Value?.UtcDateTime,
                    SessionStartedBefore = SessionStartedBefore.Value?.UtcDateTime,
                    OrganizationPersonTypes = OrganizationRole.ValuesArray,
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                if (Identity.IsOperator)
                    OrganizationIdentifier.Value = IsPostBack ? value.OrganizationIdentifier : Organization.Identifier;

                UserName.Text = value.UserEmail;
                UserHostAddress.Text = value.UserHostAddress;
                UserBrowser.Text = value.UserBrowser;
                UserLanguage.Text = value.UserLanguage;
                IsValid.ValueAsBoolean = value.SessionIsAuthenticated;
                SessionStartedSince.Value = value.SessionStartedSince;
                SessionStartedBefore.Value = value.SessionStartedBefore;
                OrganizationRole.Values = value.OrganizationPersonTypes;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            SetCheckAll(OrganizationRole, "Organization Role");

            OrganizationIdentifier.Enabled = Identity.IsOperator;
            OrganizationIdentifier.Value = Organization.Identifier;
        }

        public override void Clear()
        {
            if (Identity.IsOperator)
                OrganizationIdentifier.Value = null;

            UserName.Text = null;
            UserHostAddress.Text = null;
            UserBrowser.Text = null;
            UserLanguage.Text = null;
            IsValid.ClearSelection();
            SessionStartedSince.Value = null;
            SessionStartedBefore.Value = null;
            OrganizationRole.ClearSelection();
        }
    }
}