using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Commands;

using InSite.Application.Achievements.Write;
using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class Expiration : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? AchievementIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/records/achievements/outline?id={AchievementIdentifier}&panel=setup";

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

            var achievement = AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value) : null;
            if (achievement == null)
                HttpResponseHelper.Redirect($"/ui/admin/records/achievements/search", true);

            if(!achievement.AchievementIsEnabled)
            {
                SaveButton.Visible = false;
                EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
            }

            PageHelper.AutoBindHeader(this, null, achievement.AchievementTitle);

            AchievementDetails.BindAchievement(achievement, User.TimeZone, false);
            ExpirationField.SetExpiration(achievement);

            CancelButton.NavigateUrl = OutlineUrl;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var expiration = ExpirationField.GetExpiration();
            var commands = new List<Command>
            {
                new ChangeAchievementExpiry(AchievementIdentifier.Value, expiration)
            };

            if (BulkUpdateCredentials.SelectedValue == "Yes")
                ChangeCredentialExpiration(expiration, commands);

            ServiceLocator.SendCommands(commands);

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private void ChangeCredentialExpiration(Domain.Records.Expiration achievementExpiration, List<Command> commands)
        {
            var credentials = ServiceLocator.AchievementSearch.GetCredentials(new VCredentialFilter
            {
                AchievementIdentifier = AchievementIdentifier.Value
            });

            foreach (var credential in credentials)
            {
                var credentialExpiration = new Expiration();
                if (!credential.CredentialExpirationType.HasValue() || !credentialExpiration.Equals(achievementExpiration))
                    commands.Add(new ChangeCredentialExpiration(credential.CredentialIdentifier, achievementExpiration));
            }
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={AchievementIdentifier}&panel=setup"
                : null;
        }
    }
}
