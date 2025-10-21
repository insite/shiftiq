using System;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Admin.Achievements.Credentials.Controls
{
    public partial class CredentialDetails : System.Web.UI.UserControl
    {
        public void BindCredential(VCredential credential, TimeZoneInfo tz, bool showGranted = true, bool showRevoked = true)
        {
            var achievement = ServiceLocator.AchievementSearch.GetAchievement(credential.AchievementIdentifier);
            AchievementDetail.BindAchievement(achievement, tz);

            var person = ServiceLocator.PersonSearch.GetPerson(credential.UserIdentifier, credential.OrganizationIdentifier);
            var user = ServiceLocator.UserSearch.GetUser(credential.UserIdentifier);
            PersonDetail.BindPerson(person, user, tz);

            GrantedDiv.Visible = showGranted;
            RevokedDiv.Visible = showRevoked;

            if (credential.CredentialAssigned.HasValue)
                Assigned.Text = GetLocalTime(credential.CredentialAssigned, tz);
            else
                AssignedDiv.Visible = false;
            if (credential.CredentialGranted.HasValue)
                Granted.Text = GetLocalTime(credential.CredentialGranted, tz);
            else
                GrantedDiv.Visible = false;
            if (credential.CredentialRevoked.HasValue)
                Revoked.Text = GetLocalTime(credential.CredentialRevoked, tz);
            else
                RevokedDiv.Visible = false;
            if (credential.CredentialExpirationExpected.HasValue)
                Expired.Text = GetLocalTime(credential.CredentialExpirationExpected, tz);
            else
                ExpiredDiv.Visible = false;
        }

        private string GetLocalTime(DateTimeOffset? item, TimeZoneInfo tz)
        {
            return item.Format(tz, nullValue: "None");
        }
    }
}