using InSite.Common.Web.UI;
using InSite.Persistence;

namespace InSite.Admin.Accounts.Users.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<UserFilter>
    {
        public override UserFilter Filter
        {
            get
            {
                var filter = new UserFilter
                {
                    ContactName = Name.Text,
                    EmailContains = Email.Text,
                    IsEmailValid = IsEmailValid.ValueAsBoolean,
                    EmailVerified = EmailVerified.ValueAsBoolean,
                    LastAuthenticated = new Shift.Common.DateTimeOffsetRange
                    {
                        Since = LastAuthenticatedSince.Value,
                        Before = LastAuthenticatedBefore.Value
                    },
                    IsAccessGranted = IsAccessGranted.ValueAsBoolean,
                    IsLicensed = IsLicensed.ValueAsBoolean,
                    AccessGranted = new Shift.Common.DateTimeOffsetRange
                    {
                        Since = UserAccessGrantedSince.Value,
                        Before = UserAccessGrantedBefore.Value
                    },

                    CompanyName = CompanyName.Text,
                    OrganizationStatus = OrganizationStatus.Value,
                    UserSessionStatus = UserSessionStatus.Value,

                    AgentOrganizationIdentifier = Organization.Identifier
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                Name.Text = value.ContactName;
                Email.Text = value.EmailContains;
                IsEmailValid.ValueAsBoolean = value.IsEmailValid;
                EmailVerified.ValueAsBoolean = value.EmailVerified;
                IsAccessGranted.ValueAsBoolean = value.IsAccessGranted;
                IsLicensed.ValueAsBoolean = value.IsLicensed;

                LastAuthenticatedSince.Value = value.LastAuthenticated?.Since;
                LastAuthenticatedBefore.Value = value.LastAuthenticated?.Before;

                UserAccessGrantedSince.Value = value.AccessGranted?.Since;
                UserAccessGrantedBefore.Value = value.AccessGranted?.Before;
                CompanyName.Text = value.CompanyName;
                OrganizationStatus.Value = value.OrganizationStatus;
                UserSessionStatus.Value = value.UserSessionStatus;
            }
        }

        public override void Clear()
        {
            Name.Text = string.Empty;
            Email.Text = string.Empty;
            IsEmailValid.ClearSelection();
            EmailVerified.ClearSelection();
            LastAuthenticatedSince.Value = null;
            LastAuthenticatedBefore.Value = null;
            IsAccessGranted.ClearSelection();
            IsLicensed.ClearSelection();
            UserAccessGrantedSince.Value = null;
            UserAccessGrantedBefore.Value = null;
            CompanyName.Text = string.Empty;
            OrganizationStatus.ClearSelection();
            UserSessionStatus.ClearSelection();
        }
    }
}