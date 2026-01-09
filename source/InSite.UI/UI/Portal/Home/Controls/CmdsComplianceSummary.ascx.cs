using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.CMDS.Common.Models;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class CmdsComplianceSummary : UserControl
    {
        public IEnumerable<SummaryDataItem> DataItems { get; private set; }

        #region Public methods

        public void BindModelToControls(Guid employeeId, Guid organizationId)
        {
            var items = new List<SummaryDataItem>();
            var compliance = UserCompetencyRepository.SelectComplianceSummary(employeeId, organizationId, false);

            AddTimeSensitiveAchievements(employeeId, organizationId, items);
            AddAdditionalComplianceRequirements(employeeId, organizationId, items);
            AddCompetencyStatus(compliance, null, items);
            AddEmployeeAchievementScores(employeeId, organizationId, items);

            Repeater.DataSource = items;
            Repeater.DataBind();
        }

        public void BindModelToControls(Guid employeeId, Guid organizationId, Guid profileId)
        {
            var items = new List<SummaryDataItem>();
            var compliance = UserCompetencyRepository.SelectComplianceSummary(employeeId, organizationId, true);

            AddTimeSensitiveAchievements(employeeId, organizationId, items);
            AddAdditionalComplianceRequirements(employeeId, organizationId, items);
            AddCompetencyStatus(compliance, profileId, items);
            AddEmployeeAchievementScores(employeeId, organizationId, items);

            Repeater.DataSource = DataItems = items;
            Repeater.DataBind();
        }

        #endregion

        #region Loading

        private static decimal CalculateScore(int a, int b)
        {
            var score =
                b != 0
                    ? (decimal)100 * a / b
                    : 100;

            return Math.Round(score, 1);
        }

        private void AddTimeSensitiveAchievements(Guid employeeId, Guid organization, List<SummaryDataItem> list)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(employeeId, organization, AchievementTypes.TimeSensitiveSafetyCertificate, Shift.Constant.InclusionType.Only, out var completed, out var total);

            var item = new SummaryDataItem
            {
                Title = "Time-Sensitive Safety Certificates",
                Completed = completed,
                Total = total,
                ProgressText = string.Format("{0} out of {1} completed", completed, total),
                ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(completed, total)) : "-",
                ProgressUrl = string.Format(
                    "/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.TimeSensitiveSafetyCertificate))
            };

            if (completed == total)
            {

                item.FlagType = CmdsFlagType.Green;
                item.FlagTooltip = "Compliant";
            }
            else
            {
                item.FlagType = CmdsFlagType.Red;
                item.FlagTooltip = "Not Compliant";
            }

            list.Add(item);
        }

        private void AddAdditionalComplianceRequirements(Guid employeeId, Guid organizationId, List<SummaryDataItem> list)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(employeeId, organizationId, AchievementTypes.AdditionalComplianceRequirement, Shift.Constant.InclusionType.Only, out var completed, out var total);

            var item = new SummaryDataItem
            {
                Title = "Additional Compliance Requirements",
                Completed = completed,
                Total = total,
                ProgressText = string.Format("{0} out of {1} completed", completed, total),
                ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(completed, total)) : "-",
                ProgressUrl = string.Format(
                    "/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.AdditionalComplianceRequirement))
            };

            if (completed == total)
            {
                item.FlagType = CmdsFlagType.Green;
                item.FlagTooltip = "Completed";
            }
            else
            {
                item.FlagType = CmdsFlagType.Red;
                item.FlagTooltip = "Not Completed";
            }

            list.Add(item);
        }

        private static void AddCompetencyStatus(UserStatusHome compliance, Guid? profileId, List<SummaryDataItem> list)
        {
            {
                var criticalItem = Cmds.User.Achievements.Controls.ComplianceSummary.GetCompetencyStatus(
                    compliance.Critical, compliance.CriticalValidated, compliance.CriticalSubmitted, profileId, "Critical");

                criticalItem.Title = "Critical Competencies";
                criticalItem.Completed = compliance.CriticalValidated;
                criticalItem.Total = compliance.Critical;

                list.Add(criticalItem);
            }

            {
                var nonCriticalItem = Cmds.User.Achievements.Controls.ComplianceSummary.GetCompetencyStatus(
                    compliance.NonCritical, compliance.NonCriticalValidated, compliance.NonCriticalSubmitted, profileId, "Non-Critical");

                nonCriticalItem.Title = "Non-Critical Competencies";
                nonCriticalItem.Completed = compliance.NonCriticalValidated;
                nonCriticalItem.Total = compliance.NonCritical;

                list.Add(nonCriticalItem);
            }
        }

        private void AddEmployeeAchievementScores(Guid employeeId, Guid organizationId, List<SummaryDataItem> list)
        {
            var organization = ServiceLocator.OrganizationSearch.GetModel(organizationId);

            var copItem = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.CodeOfPractice, organization.Code),
                ProgressText = "0 out of 0 completed",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.CodeOfPractice))
            };
            var sopItem = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.SafeOperatingPractice, organization.Code),
                ProgressText = "0 out of 0 completed",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.SafeOperatingPractice))
            };
            var hrdItem = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.HumanResourcesDocument, organization.Code),
                ProgressText = "0 out of 0 completed",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.HumanResourcesDocument))
            };
            var hrmItem = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.HumanResourcesModule, organization.Code),
                ProgressText = "0 out of 0 completed",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.HumanResourcesModule))
            };

            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(employeeId, organizationId, false, true, new[]
            {
                AchievementTypes.CodeOfPractice,
                AchievementTypes.SafeOperatingPractice,
                AchievementTypes.HumanResourcesDocument,
                AchievementTypes.HumanResourcesModule
            });

            foreach (var row in table)
            {
                var type = row.AchievementLabel;
                var total = row.Total;
                var signed = row.Signed;

                SummaryDataItem item = null;

                if (StringHelper.Equals(type, AchievementTypes.CodeOfPractice))
                    item = copItem;
                else if (StringHelper.Equals(type, AchievementTypes.SafeOperatingPractice))
                    item = sopItem;
                else if (StringHelper.Equals(type, AchievementTypes.HumanResourcesDocument))
                    item = hrdItem;
                else if (StringHelper.Equals(type, AchievementTypes.HumanResourcesModule))
                    item = hrmItem;

                if (item == null)
                    continue;

                item.Completed = signed;
                item.Total = total;
                item.ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(signed, total)) : "-";
                item.ProgressText = string.Format("{0} out of {1} signed off", signed, total);

                if (signed == total)
                {
                    item.FlagType = CmdsFlagType.Green;
                    item.FlagTooltip = "Compliant";
                }
                else
                {
                    item.FlagType = CmdsFlagType.Red;
                    item.FlagTooltip = "Not Compliant";
                }
            }

            list.Add(copItem);
            list.Add(sopItem);

            var show = organization.Toolkits.Achievements?.ShowAchievementsInComplianceSummary ?? false;

            if (show)
            {
                AddAchievements(employeeId, organizationId, list);

                list.Add(hrmItem);
            }
            else
            {
                list.Add(hrdItem);
            }
        }

        private void AddAchievements(Guid employeeId, Guid organization, List<SummaryDataItem> list)
        {
            VCmdsCredentialSearch.CountCompletedByStatus(employeeId, organization, AchievementTypes.Module, Shift.Constant.InclusionType.Only, out var completed, out var total);

            var item = new SummaryDataItem
            {
                Title = "e-Learning Modules",
                Completed = completed,
                Total = total,
                ProgressText = string.Format("{0} out of {1} completed", completed, total),
                ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(completed, total)) : "-",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}", employeeId,
                    HttpUtility.UrlEncode(AchievementTypes.Module))
            };

            if (completed == total)
            {
                item.FlagType = CmdsFlagType.Green;
                item.FlagTooltip = "Completed";
            }
            else
            {
                item.FlagType = CmdsFlagType.Red;
                item.FlagTooltip = "Not Completed";
            }

            list.Add(item);
        }

        #endregion
    }
}