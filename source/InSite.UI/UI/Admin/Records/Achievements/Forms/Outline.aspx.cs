using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using InSite.Admin.Achievements.Achievements.Controls;
using InSite.Application.Achievements.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;

using AggregateOutline = InSite.Admin.Logs.Aggregates.Outline;

namespace InSite.Admin.Achievements.Achievements.Forms
{
    public partial class Outline : AdminBasePage
    {
        private Guid? AchievementID => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        private string DefaultPanel => Request.QueryString["panel"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LockButton.Click += LockButton_Click;
            UnlockButton.Click += UnlockButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            LoadData();

            if (DefaultPanel == "setup")
                AchievementSetupTab.IsSelected = true;
        }

        private void LoadData()
        {
            if (AchievementID == null)
            {
                HttpResponseHelper.Redirect("/ui/admin/records/achievements/search");
                return;
            }

            var achievement = ServiceLocator.AchievementSearch.GetAchievement(AchievementID.Value);
            var organization = achievement != null ? OrganizationSearch.Select(achievement.OrganizationIdentifier) : null;

            if (achievement == null
                    || (
                        achievement.OrganizationIdentifier != CurrentSessionState.Identity.Organization.OrganizationIdentifier
                        && !ServiceLocator.Partition.IsE03()
                    )
                )
            {
                HttpResponseHelper.Redirect("/ui/admin/records/achievements/search");
                return;
            }

            PageHelper.AutoBindHeader(this, qualifier: achievement.AchievementTitle);

            AchievementIdentifier.Text = achievement.AchievementIdentifier.ToString();

            AchievementStatus.Text = achievement.AchievementIsEnabled
                ? "<span class='badge bg-success'><i class='far fa-lock-open'></i> Unlocked</span>"
                : "<span class='badge bg-danger'><i class='far fa-lock'></i> Locked</span>";

            BindModelToControls(ServiceLocator.AchievementSearch.GetAchievement(AchievementID.Value, x => x.Prerequisites).Prerequisites);

            LockButton.Visible = DeleteLink.Visible = achievement.AchievementIsEnabled;
            UnlockButton.Visible = !achievement.AchievementIsEnabled;

            AchievementTitle.Text = achievement.AchievementTitle;
            AchievementLabel.Text = achievement.AchievementLabel;
            AchievementDescription.Text = achievement.AchievementDescription;

            AchievementExpiry.Text = GetExpiryText(achievement);
            CertificateLayout.Text = achievement.CertificateLayoutCode.IfNullOrEmpty("None");
            AchievementIsReported.Text = achievement.AchievementReportingDisabled ? "Disabled" : "Enabled";
            AchievementType.Text = achievement.AchievementType.IfNullOrEmpty("None");
            AchievementAllowSelfDeclared.Text = achievement.AchievementAllowSelfDeclared ? "Allow" : "Disallow";

            if (achievement.BadgeImageUrl.HasValue())
                AchievementBadge.Text = $"<img src='{achievement.BadgeImageUrl}' alt='SVG Image' />";

            DescribeTitle.Visible = achievement.AchievementIsEnabled;
            DescribeLabel.Visible = achievement.AchievementIsEnabled;
            DescribeDescription.Visible = achievement.AchievementIsEnabled;
            ChangeExpiration.Visible = achievement.AchievementIsEnabled;
            ChangeCertificateLayout.Visible = achievement.AchievementIsEnabled;
            ChangeAchievementType.Visible = achievement.AchievementIsEnabled;
            BadgePanel.Visible = achievement.HasBadgeImage == true;

            DescribeTitle.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            DescribeLabel.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            DescribeDescription.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            DescribeBadge.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            ChangeCertificateLayout.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            ChangeExpiration.NavigateUrl = $"/ui/admin/records/achievements/expiration?id={AchievementID}";
            AchievementIsReportedLink.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            ChangeAchievementType.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";
            AchievementAllowSelfDeclaredLink.NavigateUrl = $"/ui/admin/records/achievements/describe?id={AchievementID}";

            AchievementTypeField.Visible = !string.IsNullOrEmpty(achievement.AchievementType) ||
                TCollectionItemCache.Exists(new TCollectionItemFilter
                {
                    CollectionName = CollectionName.Achievements_Templates_Types_Name,
                    OrganizationIdentifier = Organization.OrganizationIdentifier
                });

            DepartmentChecklist.LoadAchievements(achievement.AchievementIdentifier);

            CredentialGrid.LoadDataByAchievementID(AchievementID.Value, true, null);

            ViewHistoryLink.NavigateUrl = AggregateOutline.GetUrl(AchievementID.Value, $"/ui/admin/records/achievements/outline?id={AchievementID}");
            DeleteLink.NavigateUrl = $"/ui/admin/records/achievements/delete?id={AchievementID}";
        }

        private void BindModelToControls(IEnumerable<QAchievementPrerequisite> prerequisites)
        {
            if (!prerequisites.Any())
                return;

            var prerequesitesAndCondictions = prerequisites
                .GroupBy(x => x.PrerequisiteIdentifier)
                .Select(x => new
                {
                    PrerequisiteIdentifier = x.Key,
                    Conditions = x.Select(p => p.PrerequisiteAchievementIdentifier).ToList()
                })
                .ToList();

            var html = new StringBuilder();
            var i = 0;

            html.Append("<div class='achievement-prerequisite'>");
            foreach (var prerequisiteAndConditions in prerequesitesAndCondictions)
            {
                var j = 0;

                html.Append($"<div class='float-end'><a href='/ui/admin/records/achievements/delete-prerequisite?achievement={AchievementID}&prerequisite={prerequisiteAndConditions.PrerequisiteIdentifier}'><i class='fas fa-trash-alt'></i></a></div>");
                html.Append("<ul class='achievement-prerequisite-conditions'>");
                foreach (var condition in prerequisiteAndConditions.Conditions)
                {
                    var achievement = ServiceLocator.AchievementSearch.GetAchievement(condition);
                    if (achievement == null)
                        continue;

                    html.Append($"<li>{achievement.AchievementTitle}");

                    if (++j < prerequisiteAndConditions.Conditions.Count)
                        html.Append("<div class='text-primary'>&ndash; or &ndash;</div>");

                    html.Append("</li>");
                }
                html.Append("</ul>");

                if (++i < prerequesitesAndCondictions.Count)
                    html.Append("<div class='text-success'>&mdash; <strong>AND</strong> &mdash;</div>");
            }
            html.Append("</div>");
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new LockAchievement(AchievementID.Value));

            LoadData();

            StatusAlert.AddMessage(AlertType.Success, "Achievement is locked");
        }

        private void UnlockButton_Click(object sender, EventArgs e)
        {
            ServiceLocator.SendCommand(new UnlockAchievement(AchievementID.Value));

            LoadData();

            StatusAlert.AddMessage(AlertType.Success, "Achievement is unlocked");
        }

        #region Helper methods

        private string GetExpiryText(QAchievement i)
        {
            var achievementExpiration = new Domain.Records.Expiration(
                i.ExpirationType, i.ExpirationFixedDate,
                i.ExpirationLifetimeQuantity, i.ExpirationLifetimeUnit);

            return achievementExpiration.ToString(User.TimeZone);
        }


        protected string GetLabelContentText()
        {
            return "Test";
        }

        #endregion
    }
}