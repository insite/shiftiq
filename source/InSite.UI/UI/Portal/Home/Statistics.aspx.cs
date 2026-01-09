using System;
using System.Linq;

using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

namespace InSite.UI.Portal.Home
{
    public partial class Statistics : PortalBasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ShowWhatPanel.Visible = ServiceLocator.Partition.IsE03();
            ShowWhat.AutoPostBack = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            BindLearningPlan();
            BindAchievements();

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);
        }

        private void BindLearningPlan()
        {
            var achievements = VCmdsCredentialSearch.SelectForTrainingPlan(User.Identifier, Organization.OrganizationIdentifier, null);
            var y = achievements.Count;

            if (y > 0)
            {
                var x = achievements.Count(a => a.CredentialStatus == "Valid");
                var percent = (decimal)x / y;
                LearningPlanProgressPercent.InnerText = $"{percent:p0}";
                LearningPlanProgressPoints.InnerHtml = $"<i class='far fa-flag-checkered me-1'></i> {x} of {y} Achievements";
            }
            else
            {
                LearningPlanProgressPercent.InnerText = "N/A";
                LearningPlanProgressPoints.InnerText = "No Achievements Planned";
            }
        }

        private void BindAchievements()
        {
            var filter = new Application.Records.Read.VCredentialFilter { UserIdentifier = User.Identifier, OrganizationIdentifier = Organization.Identifier };
            filter.CredentialStatus = "Valid";
            var valid = ServiceLocator.AchievementSearch.CountCredentials(filter);
            filter.CredentialStatus = "Expired";
            var expired = ServiceLocator.AchievementSearch.CountCredentials(filter);

            AchievementsValid.InnerText = $"{valid:n0}";
            AchievementsExpired.InnerHtml = $"<i class='far fa-alarm-clock me-1'></i> {expired:n0} Expired";
        }
    }
}