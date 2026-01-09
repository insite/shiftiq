using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Custom.CMDS.Common.Controls.Server;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.CMDS.Common.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.Cmds.User.Achievements.Controls
{
    public partial class ComplianceSummary : UserControl
    {
        private void CustomizeForKeyera()
        {
            var organization = CurrentSessionState.Identity.Organization.Code;
            var isKeyera = organization == "keyera";

            ElmLink.Visible = isKeyera;
            ElmRow.Visible = isKeyera;

            CopTitle.Text = AchievementTypes.Pluralize(AchievementTypes.CodeOfPractice, organization);
            SopTitle.Text = AchievementTypes.Pluralize(AchievementTypes.SafeOperatingPractice, organization);
            HrdTitle.Text = AchievementTypes.Pluralize(AchievementTypes.HumanResourcesDocument, organization);
            HrdRow.Visible = !isKeyera;
        }

        #region Public methods

        public void LoadData(Guid employeeId, Guid organizationId)
        {
            CustomizeForKeyera();

            LoadTimeSensitiveAchievements(employeeId, organizationId);
            LoadAdditionalComplianceRequirements(employeeId, organizationId);
            LoadEmployeeAchievementScores(employeeId, organizationId);
            LoadAchievements(employeeId, organizationId);

            var compliance = UserCompetencyRepository.SelectComplianceSummary(employeeId, organizationId, false);
            // LoadCompetencies(compliance);

            // var profile = UserProfileRepository.SelectPrimaryProfile(employeeID, organizationId)?.ProfileStandardIdentifier;
            Guid? profile = null;
            SetCompetencyStatus(CriticalCompetencyLink, CriticalCompetencyText, CriticalCompetencyPercent, CriticalCompetencyImage, compliance.Critical, compliance.CriticalValidated, compliance.CriticalSubmitted, profile, "Critical");
            SetCompetencyStatus(NonCriticalCompetencyLink, NonCriticalCompetencyText, NonCriticalCompetencyPercent, NonCriticalCompetencyImage, compliance.NonCritical, compliance.NonCriticalValidated, compliance.NonCriticalSubmitted, profile, "Non-Critical");

            SetLinks(employeeId);
        }

        #endregion

        #region Loading

        private void SetLinks(Guid employeeID)
        {
            TimeSensitiveAchievementLink.NavigateUrl = string.Format(
                "/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                HttpUtility.UrlEncode(AchievementTypes.TimeSensitiveSafetyCertificate));
            AdditionalComplianceRequirementsLink.NavigateUrl = string.Format(
                "/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                HttpUtility.UrlEncode(AchievementTypes.AdditionalComplianceRequirement));
            COPLink.NavigateUrl =
                string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                    HttpUtility.UrlEncode(AchievementTypes.CodeOfPractice));
            SOPLink.NavigateUrl =
                string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                    HttpUtility.UrlEncode(AchievementTypes.SafeOperatingPractice));
            HrdLink.NavigateUrl =
                string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                    HttpUtility.UrlEncode(AchievementTypes.HumanResourcesDocument));
            ElmLink.NavigateUrl =
                string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeID,
                    HttpUtility.UrlEncode(AchievementTypes.Module));
        }

        private static decimal CalculateScore(int a, int b)
        {
            var score =
                b != 0
                    ? (decimal)100 * a / b
                    : 100;

            return Math.Round(score, 1);
        }

        private void LoadAchievements(Guid userID, Guid organization)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(userID, organization, AchievementTypes.Module, Shift.Constant.InclusionType.Only, out int completed, out int total);

            ElmPercent.Text = string.Format("{0:n1}%", CalculateScore(completed, total));
            ElmText.Text = string.Format("{0} out of {1} completed", completed, total);

            ElmImage.Type = completed == total ? CmdsFlagType.Green : CmdsFlagType.Red;
            ElmImage.ToolTip = completed == total ? "Completed" : "Not Completed";
        }

        private void LoadTimeSensitiveAchievements(Guid userID, Guid organization)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(userID, organization, AchievementTypes.TimeSensitiveSafetyCertificate, Shift.Constant.InclusionType.Only, out var completed, out var total);

            TimeSensitiveAchievementPercent.Text = string.Format("{0:n1}%", CalculateScore(completed, total));
            TimeSensitiveAchievementText.Text = string.Format("{0} out of {1} completed", completed, total);

            if (completed == total)
            {
                TimeSensitiveAchievementImage.Type = CmdsFlagType.Green;
                TimeSensitiveAchievementImage.ToolTip = "Compliant";
            }
            else
            {
                TimeSensitiveAchievementImage.Type = CmdsFlagType.Red;
                TimeSensitiveAchievementImage.ToolTip = "Not Compliant";
            }

            TimeSensitiveAchievementImage.ToolTip = TimeSensitiveAchievementImage.ToolTip;
        }

        private void LoadAdditionalComplianceRequirements(Guid userID, Guid organization)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(userID, organization, AchievementTypes.AdditionalComplianceRequirement, Shift.Constant.InclusionType.Only, out int completed, out int total);

            AdditionalComplianceRequirementsPercent.Text = string.Format("{0:n1}%", CalculateScore(completed, total));
            AdditionalComplianceRequirementsText.Text = string.Format("{0} out of {1} completed", completed, total);

            if (completed == total)
            {
                AdditionalComplianceRequirementsImage.Type = CmdsFlagType.Green;
                AdditionalComplianceRequirementsImage.ToolTip = "Completed";
            }
            else
            {
                AdditionalComplianceRequirementsImage.Type = CmdsFlagType.Red;
                AdditionalComplianceRequirementsImage.ToolTip = "Not Completed";
            }

            AdditionalComplianceRequirementsImage.ToolTip = AdditionalComplianceRequirementsImage.ToolTip;
        }

        public static void SetCompetencyStatus(HyperLink link, Literal textLabel, Literal percentLabel, Flag flagImage, long total, long validated, long submitted, Guid? profileId, string criticality)
        {
            var item = GetCompetencyStatus(total, validated, submitted, profileId, criticality);

            var hasUrl = item.ProgressUrl != null;
            var hasFlag = item.FlagType.HasValue;

            flagImage.Visible = hasFlag;
            if (hasFlag)
            {
                flagImage.Type = item.FlagType.Value;
                flagImage.ToolTip = item.FlagTooltip;
            }

            link.Visible = hasUrl;
            textLabel.Visible = !hasUrl;

            if (hasUrl)
            {
                link.Text = item.ProgressText;
                link.NavigateUrl = item.ProgressUrl;
            }
            else
            {
                textLabel.Text = item.ProgressText;
            }

            percentLabel.Text = item.ProgressPercent;
        }

        public static SummaryDataItem GetCompetencyStatus(long total, long validated, long submitted, Guid? profileId, string criticality)
        {
            var result = new SummaryDataItem
            {
                ProgressPercent = string.Format("{0:n1}%", CalculateScore(unchecked((int)validated), unchecked((int)total))),
                ProgressText = string.Format("{0} out of {1} validated", validated, total),
                ProgressUrl = total != validated || total != 0
                    ? $"/ui/cmds/portal/validations/competencies/search" +
                      $"?{(profileId.HasValue ? "profile=" + profileId : "compliance=1")}" +
                      $"&criticality={criticality}" +
                      $"&validated={(total == validated ? "true" : "false")}"
                    : null
            };

            if (total > 0)
            {
                if (total == validated)
                {
                    result.FlagType = CmdsFlagType.Green;
                    result.FlagTooltip = "Compliant";
                }
                else
                {
                    result.FlagType = total == validated + submitted ? CmdsFlagType.Yellow : CmdsFlagType.Red;
                    result.FlagTooltip = total == validated + submitted
                        ? "Not complaint, but submitted for validation"
                        : "Not compliant";
                }
            }

            return result;
        }

        private void LoadEmployeeAchievementScores(Guid userID, Guid organization)
        {
            CopImage.Visible = false;
            SopImage.Visible = false;
            HrdImage.Visible = false;

            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(userID, organization, false, true,
                new[]
                {
                    AchievementTypes.CodeOfPractice, AchievementTypes.SafeOperatingPractice,
                    AchievementTypes.HumanResourcesDocument
                });

            foreach (var row in table)
            {
                var type = row.AchievementLabel;
                var total = row.Total;
                var signed = row.Signed;

                var percent = string.Format("{0:n1}%", CalculateScore(signed, total));
                var text = string.Format("{0} out of {1} signed off", signed, total);

                Flag image = null;

                if (StringHelper.Equals(type, AchievementTypes.CodeOfPractice))
                {
                    COPPercent.Text = percent;
                    COPText.Text = text;
                    CopImage.Visible = true;
                    image = CopImage;
                }
                else if (StringHelper.Equals(type, AchievementTypes.SafeOperatingPractice))
                {
                    SOPPercent.Text = percent;
                    SOPText.Text = text;
                    SopImage.Visible = true;
                    image = SopImage;
                }
                else if (StringHelper.Equals(type, AchievementTypes.HumanResourcesDocument))
                {
                    HrdPercent.Text = percent;
                    HrdText.Text = text;
                    HrdImage.Visible = true;
                    image = HrdImage;
                }

                if (image != null)
                {
                    if (signed == total)
                    {
                        image.Type = CmdsFlagType.Green;
                        image.ToolTip = "Compliant";
                    }
                    else
                    {
                        image.Type = CmdsFlagType.Red;
                        image.ToolTip = "Not Compliant";
                    }

                    image.ToolTip = image.ToolTip;
                }
            }
        }

        #endregion
    }
}