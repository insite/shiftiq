using System;

using Humanizer;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Achievements.Credentials.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Guid? CredentialID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            var credential = CredentialID.HasValue ? ServiceLocator.AchievementSearch.GetCredential(CredentialID.Value) : null;
            var organization = credential != null ? OrganizationSearch.Select(credential.OrganizationIdentifier) : null;

            if (credential == null
                    || (
                        credential.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier
                        && !ServiceLocator.Partition.IsE03()
                    )
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/credentials/search");
                return;
            }

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{credential.AchievementTitle} <span class=\"form-text\" style=\"padding-right:10px;\">for</span> {credential.UserFullName}");

            AchievementTitle.Text = credential.AchievementTitle;
            AchievementLabel.Text = credential.AchievementLabel;
            Achievementlink.HRef = $"/ui/admin/records/achievements/outline?id={credential.AchievementIdentifier}";
            AchievementDescription.Text = credential.AchievementDescription;
            AchievementExpiry.Text = GetAchievementExpiryText(credential);
            AchievementStatus.Text = credential.AchievementIsEnabled ? "Unlocked" : "Locked";

            AuthorityName.Text = credential.AuthorityName ?? "None";
            AuthorityType.Text = credential.AuthorityType ?? "None";
            AuthorityLocation.Text = credential.AuthorityLocation ?? "None";
            AuthorityReference.Text = credential.AuthorityReference ?? "None";

            UserName.Text = credential.UserFullName;
            UserLink.HRef = $"/ui/admin/contacts/people/edit?contact={credential.UserIdentifier}";
            UserEmail.Text = $"<a href='mailto:{credential.UserEmail}'>{credential.UserEmail}</a>";
            UserEmailStatus.Text = credential.UserEmailEnabled ? "Enabled" : "Disabled";
            UserRegion.Text = credential.UserRegion ?? "None";
            UserArchived.Text = GetLocalTime(credential.UserArchived);
            UserAccessField.Visible = CurrentSessionState.Identity.User.AccessGrantedToCmds;
            UserAccess.Text = credential.UserAccessGrantedToCmds ? "Granted to CMDS" : "Not Granted to CMDS";

            CredentialIdentifier.Text = credential.CredentialIdentifier.ToString();
            CredentialStatus.Text = credential.CredentialStatus ?? "None";
            CredentialDescription.Text = credential.CredentialDescription ?? "None";
            CredentialHours.Text = credential.CredentialHours != null ? credential.CredentialHours.Value.ToString("n2") : "None";
            CredentialExpiry.Text = GetCredentialExpiryText(credential);
            CredentialNecessity.Text = credential.CredentialNecessity.HasValue() ? credential.CredentialNecessity : "None";
            CredentialPriority.Text = credential.CredentialPriority.HasValue() ? credential.CredentialPriority : "None";
            CredentialAssigned.Text = GetLocalTime(credential.CredentialAssigned);
            CredentialGranted.Text = GetLocalTime(credential.CredentialGranted);
            CredentialRevoked.Text = GetLocalTime(credential.CredentialRevoked);
            RevokedReasonField.Visible = credential.CredentialRevoked.HasValue;

            if (credential.CredentialRevoked.HasValue)
                CredentialRevokedReason.Text = credential.CredentialRevokedReason;

            CredentialExpired.Text = GetLocalTime(credential.CredentialExpirationExpected);


            EmployerName.Text = credential.EmployerGroupIdentifier.HasValue ? $"<a href=\"/ui/admin/contacts/groups/edit?contact={credential.EmployerGroupIdentifier}\">{credential.EmployerGroupName}</a>" : "None";
            OrganizationAccessField.Visible = CurrentSessionState.Identity.User.AccessGrantedToCmds;

            OrganizationAccess.Text = credential.OrganizationIdentifier == OrganizationIdentifiers.CMDS
                    || credential.ParentOrganizationIdentifier == OrganizationIdentifiers.CMDS
                ? "Granted to CMDS"
                : "Not Granted to CMDS";

            CredentialExpiryLink.NavigateUrl = $"/ui/admin/records/credentials/configure?id={CredentialID}";
            CredentialNecessityLink.NavigateUrl = $"/ui/admin/records/credentials/configure?id={CredentialID}";
            CredentialPriorityLink.NavigateUrl = $"/ui/admin/records/credentials/configure?id={CredentialID}";

            GrantCredential.NavigateUrl = $"/ui/admin/records/credentials/grant?id={CredentialID}";
            RevokeCredential.NavigateUrl = $"/ui/admin/records/credentials/revoke?id={CredentialID}";

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(CredentialID.Value, $"/ui/admin/records/credentials/outline?id={CredentialID}");
            DeleteLink.NavigateUrl = $"/ui/admin/records/credentials/delete?id={CredentialID}";

            if (HasCertificateToDownload(credential.CredentialStatus, credential.AchievementCertificateLayoutCode))
            {
                DownloadLink.Visible = true;
                DownloadLink.NavigateUrl = $"/ui/portal/records/credentials/certificate?credential={CredentialID}";
            }
        }

        #region Helper methods

        private string GetAchievementExpiryText(VCredential credential)
        {
            if (credential.AchievementExpirationType == "Fixed")
                return GetLocalTime(credential.AchievementExpirationFixedDate);

            if (credential.AchievementExpirationType == "Relative" && credential.AchievementExpirationLifetimeQuantity.HasValue && credential.AchievementExpirationLifetimeUnit.HasValue())
                return credential.AchievementExpirationLifetimeUnit.ToQuantity(credential.AchievementExpirationLifetimeQuantity.Value);

            return "Never";
        }

        private string GetCredentialExpiryText(VCredential credential)
        {
            if (credential.CredentialExpirationType == "Fixed")
                return GetLocalTime(credential.CredentialExpirationFixedDate);

            if (credential.CredentialExpirationType == "Relative" && credential.CredentialExpirationLifetimeQuantity.HasValue && credential.CredentialExpirationLifetimeUnit.HasValue())
                return credential.CredentialExpirationLifetimeUnit.ToQuantity(credential.CredentialExpirationLifetimeQuantity.Value);

            return "Never";
        }

        private string GetLocalTime(DateTimeOffset? item)
        {
            return item.FormatDateOnly(User.TimeZone, nullValue: "None");
        }

        private bool HasCertificateToDownload(string credentialStatus, string achievementCertificateLayoutCode)
        {
            return credentialStatus == Shift.Constant.CredentialStatus.Valid.ToString() && (ServiceLocator.Partition.IsE03() || achievementCertificateLayoutCode.IsNotEmpty());
        }

        #endregion
    }
}