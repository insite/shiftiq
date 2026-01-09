using System;
using System.Web;
using System.Web.UI;

using InSite.Persistence.Plugin.CMDS;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.Controls.User
{
    public partial class TrainingSummary : UserControl
    {
        public void LoadData(Guid user, Guid organization)
        {
            LoadModules(user);
            LoadGuides(user, organization);
            LoadProcedures(user, organization);
            LoadOrientations(user, organization);
            CustomizeForKeyera();

            MandatoryOrientationLink.NavigateUrl = RelativeUrl.PortalHomeUrl;
            OptionalOrientationLink.NavigateUrl = RelativeUrl.PortalHomeUrl;
        }

        private void LoadModules(Guid user)
        {
            VCmdsCredentialSearch.CountCompletedByScore(user, CurrentIdentityFactory.ActiveOrganizationIdentifier, AchievementTypes.Module, out var completedAchievements, out var totalAchievements);
            var score = CalculateScore(completedAchievements, totalAchievements);
            ModuleProgress.Text = string.Format("{0:n1}%", score);
            ModuleLink.Text = string.Format("{0} out of {1} completed", completedAchievements, totalAchievements);
            ModuleLink.NavigateUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", user, HttpUtility.UrlEncode(AchievementTypes.Module));
            ModuleImage.Type = totalAchievements == completedAchievements ? CmdsFlagType.Green : CmdsFlagType.Red;
            ModuleImage.ToolTip = totalAchievements == completedAchievements ? "Completed" : "Not Completed";
        }

        private void LoadGuides(Guid user, Guid organization)
        {
            GuideImage.Visible = false;
            GuideLink.NavigateUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", user, HttpUtility.UrlEncode(AchievementTypes.TrainingGuide));

            var total = 0;
            var valid = 0;

            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(user, organization, false, true, new[] { AchievementTypes.TrainingGuide });

            foreach (var row in table)
            {
                total += row.Total;
                valid += row.Signed;

                var percent = string.Format("{0:n1}%", CalculateScore(valid, total));
                var text = string.Format("{0} out of {1} completed", valid, total);

                GuideProgress.Text = percent;
                GuideLink.Text = text;
                GuideImage.Visible = true;
                GuideImage.Type = total == valid ? CmdsFlagType.Green : CmdsFlagType.Red;
                GuideImage.ToolTip = total == valid ? "Completed" : "Not Completed";
            }
        }

        private void LoadProcedures(Guid user, Guid organization)
        {
            ProcedureLink.NavigateUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", user, HttpUtility.UrlEncode(AchievementTypes.SiteSpecificOperatingProcedure));
            ProcedureImage.Visible = false;

            var total = 0;
            var valid = 0;

            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(user, organization, false, true, new[] { AchievementTypes.SiteSpecificOperatingProcedure });

            foreach (var row in table)
            {
                total += row.Total;
                valid += row.Signed;

                var percent = string.Format("{0:n1}%", CalculateScore(valid, total));
                var text = string.Format("{0} out of {1} completed", valid, total);

                ProcedureProgress.Text = percent;
                ProcedureLink.Text = text;
                ProcedureImage.Visible = true;
                ProcedureImage.Type = total == valid ? CmdsFlagType.Green : CmdsFlagType.Red;
                ProcedureImage.ToolTip = total == valid ? "Completed" : "Not Completed";
            }
        }

        private void LoadOrientations(Guid user, Guid organization)
        {
            {
                VCmdsCredentialSearch.CountOrientations(user, organization, true, out int valid, out int total);
                MandatoryOrientationProgress.Text = string.Format("{0:n1}%", CalculateScore(valid, total));
                MandatoryOrientationLink.Text = string.Format("{0} out of {1} completed", valid, total);
                MandatoryOrientationFlag.Visible = true;
                MandatoryOrientationFlag.Type = total == valid ? CmdsFlagType.Green : CmdsFlagType.Red;
                MandatoryOrientationFlag.ToolTip = total == valid ? "Valid" : "Pending or Expired";
            }
            {
                VCmdsCredentialSearch.CountOrientations(user, organization, false, out int valid, out int total);
                OptionalOrientationProgress.Text = string.Format("{0:n1}%", CalculateScore(valid, total));
                OptionalOrientationLink.Text = string.Format("{0} out of {1} completed", valid, total);
                OptionalOrientationFlag.Visible = true;
                OptionalOrientationFlag.Type = total == valid ? CmdsFlagType.Green : CmdsFlagType.Red;
                OptionalOrientationFlag.ToolTip = total == valid ? "Valid" : "Pending or Expired";
            }
        }

        private static decimal CalculateScore(int a, int b)
        {
            var score =
                b != 0
                    ? 100M * a / b
                    : 100M;

            return Math.Round(score, 1);
        }

        private void CustomizeForKeyera()
        {
            var organization = CurrentSessionState.Identity.Organization.Code;
            var isKeyera = organization == "keyera";

            ModuleLink.Visible = !isKeyera;
            ModuleRow.Visible = !isKeyera;
            MandatoryOrientationLink.Enabled = isKeyera;
            OptionalOrientationLink.Enabled = isKeyera;

            GuideTitle.Text = AchievementTypes.Pluralize(AchievementTypes.TrainingGuide, organization);
            ProcedureTitle.Text = AchievementTypes.Pluralize(AchievementTypes.SiteSpecificOperatingProcedure, organization);
        }
    }
}