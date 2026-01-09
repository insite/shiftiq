using System;

using InSite.Application.Achievements.Write;
using InSite.Application.Files.Read;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.UI;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class Describe : AdminBasePage, IHasParentLinkParameters
    {
        private Guid? AchievementIdentifier => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private string OutlineUrl => $"/ui/admin/records/achievements/outline?id={AchievementIdentifier}&panel=setup";

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += SaveButton_Click;

            CustomBadge.AutoPostBack = true;
            CustomBadge.SelectedIndexChanged += CustomBadge_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                var achievement = AchievementIdentifier.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value) : null;
                if (achievement == null)
                {
                    HttpResponseHelper.Redirect($"/ui/admin/records/achievements/search", true);
                }

                if (!achievement.AchievementIsEnabled)
                {
                    SaveButton.Visible = false;
                    EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
                }

                PageHelper.AutoBindHeader(this, null, achievement.AchievementTitle);

                AchievementTitle.Text = achievement.AchievementTitle;
                AchievementLabel.Text = achievement.AchievementLabel;
                AchievementDescription.Text = achievement.AchievementDescription;
                CustomBadge.SelectedValue = achievement.HasBadgeImage == true ? "true" : "false";

                if (achievement.CertificateLayoutCode != null)
                    CertificateLayout.Value = achievement.CertificateLayoutCode;

                AchievementIsReported.SelectedValue = achievement.AchievementReportingDisabled ? "false" : "true";
                AchievementAllowSelfDeclared.SelectedValue = achievement.AchievementAllowSelfDeclared ? "true" : "false";

                AchievementType.EnsureDataBound();
                AchievementType.Value = achievement.AchievementType;

                SetCustomBadgePanelVisibility();

                if (achievement.BadgeImageUrl.HasValue())
                    BadgeImage.Text = $"<img src='{achievement.BadgeImageUrl}' alt='SVG Image' />";


                CancelButton.NavigateUrl = OutlineUrl;
            }
        }

        private void CustomBadge_SelectedIndexChanged(object sender, EventArgs e) => SetCustomBadgePanelVisibility();

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementIdentifier.Value);

            if (achievement == null)
                return;

            if (!achievement.AchievementIsEnabled)
            {
                EditorStatus.AddMessage(AlertType.Warning, "Modifications are not permitted while the achievement is locked. Please unlock it before making any changes.");
                return;
            }

            DetectDescriptionChange(achievement);
            DetectLayoutChange(achievement);
            DetectAchievementIsReportedChange(achievement);
            DetectCustomBadgeChange(achievement);
            DetectBadgeChange(achievement);

            if (AchievementType.Items.Count > 0)
                ServiceLocator.SendCommand(new ChangeAchievementType(AchievementIdentifier.Value, AchievementType.Value));

            HttpResponseHelper.Redirect(OutlineUrl);
        }

        private void SetCustomBadgePanelVisibility()
        {
            BadgeImagePanel.Visible = bool.Parse(CustomBadge.SelectedValue);
        }

        private void DetectCustomBadgeChange(QAchievement achievement)
        {
            var customBadge = bool.Parse(CustomBadge.SelectedValue);

            if (achievement.HasBadgeImage == customBadge)
                return;

            if (customBadge)
                ServiceLocator.SendCommand(new EnableAchievementBadgeImage(AchievementIdentifier.Value));
            else
            {
                ServiceLocator.SendCommand(new DisableAchievementBadgeImage(AchievementIdentifier.Value));
                ServiceLocator.SendCommand(new ChangeAchievementBadgeImageUrl(AchievementIdentifier.Value, null));
            }
        }

        private void DetectBadgeChange(QAchievement achievement)
        {
            if (!BadgeUpload.HasFile)
                return;

            var model = BadgeUpload.SaveFile(achievement.AchievementIdentifier, FileObjectType.Badge);
            var files = ServiceLocator.StorageService.GetFileUrl(model.FileIdentifier, model.FileName, false);
            var change = new ChangeAchievementBadgeImageUrl
            (
                AchievementIdentifier.Value,
                PathHelper.GetOrganizationUrl(ServiceLocator.AppSettings.Environment, Identity.Organization.OrganizationCode, files)
            );

            var isChanged = !StringHelper.Equals(achievement.BadgeImageUrl, change.BadgeImageUrl);
            if (!isChanged)
                return;

            ServiceLocator.SendCommand(change);
        }

        private void DetectDescriptionChange(QAchievement achievement)
        {
            var allowSelfDeclared = false;

            if (bool.TryParse(AchievementAllowSelfDeclared.SelectedValue, out bool allow))
                allowSelfDeclared = allow;

            var describe = new DescribeAchievement(
                AchievementIdentifier.Value,
                AchievementLabel.Text,
                AchievementTitle.Text,
                AchievementDescription.Text,
                allowSelfDeclared);

            var isChanged = !StringHelper.Equals(achievement.AchievementTitle, describe.Title)
                || !StringHelper.Equals(achievement.AchievementLabel, describe.Label)
                || !StringHelper.Equals(achievement.AchievementDescription, describe.Description)
                || achievement.AchievementAllowSelfDeclared != describe.AllowSelfDeclared
                ;

            if (isChanged)
                ServiceLocator.SendCommand(describe);
        }

        private void DetectLayoutChange(QAchievement achievement)
        {
            var change = new ChangeCertificateLayout(
                AchievementIdentifier.Value,
                CertificateLayout.Value);

            var isChanged = !StringHelper.Equals(achievement.CertificateLayoutCode, change.Code);

            if (isChanged)
                ServiceLocator.SendCommand(change);
        }

        private void DetectAchievementIsReportedChange(QAchievement achievement)
        {
            var reported = bool.Parse(AchievementIsReported.SelectedValue);

            if (!achievement.AchievementReportingDisabled == reported)
                return;

            if (reported)
                ServiceLocator.SendCommand(new EnableAchievementReporting(AchievementIdentifier.Value));
            else
                ServiceLocator.SendCommand(new DisableAchievementReporting(AchievementIdentifier.Value));
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={AchievementIdentifier}&panel=setup"
                : null;
        }
    }
}
