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
    public partial class CmdsLearningSummary : UserControl
    {
        public IEnumerable<SummaryDataItem> DataItems { get; private set; }

        public void BindModelToControls(Guid userId, Guid organizationId)
        {
            var organization = ServiceLocator.OrganizationSearch.GetModel(organizationId);

            var items = new List<SummaryDataItem>();

            var hideModules = organization.Toolkits?.Achievements?.HideModulesInLearningSummary ?? false;

            if (!hideModules)
                AddModules(userId, organizationId, items);

            AddGuides(userId, organizationId, organization.Code, items);
            AddProcedures(userId, organizationId, organization.Code, items);

            Repeater.DataSource = DataItems = items;
            Repeater.DataBind();
        }

        private void AddModules(Guid userId, Guid organizationId, List<SummaryDataItem> list)
        {
            VCmdsCredentialSearch.CountCompletedByScore(userId, organizationId, AchievementTypes.Module, out var completedAchievements, out var totalAchievements);
            var score = CalculateScore(completedAchievements, totalAchievements);

            var item = new SummaryDataItem
            {
                Title = "e-Learning Modules",
                ProgressPercent = "-",
                ProgressText = "-",
                ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}",
                    userId, HttpUtility.UrlEncode(AchievementTypes.Module))
            };

            if (totalAchievements > 0)
            {
                item.Completed = completedAchievements;
                item.Total = totalAchievements;
                item.ProgressText = string.Format("{0} out of {1} completed", completedAchievements, totalAchievements);
                item.ProgressPercent = string.Format("{0:n1}%", score);
            }
            else
            {
                item.ProgressText = string.Format("0 out of 0 completed", completedAchievements, totalAchievements);
            }

            if (totalAchievements == completedAchievements)
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

        private void AddGuides(Guid userId, Guid organizationId, string organizationCode, List<SummaryDataItem> list)
        {
            var total = 0;
            var valid = 0;
            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(userId, organizationId, false, true, new[] { AchievementTypes.TrainingGuide });
            VCmdsCredentialSearch.PolicySignedGroup lastItem = null;

            foreach (var row in table)
            {
                total += row.Total;
                valid += row.Signed;
                lastItem = row;
            }

            var item = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.TrainingGuide, organizationCode),
            };

            if (lastItem != null)
            {
                item.Completed = valid;
                item.Total = total;
                item.ProgressPercent = string.Format("{0:n1}%", CalculateScore(valid, total));
                item.ProgressText = string.Format("{0} out of {1} completed", valid, total);
                item.ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}",
                    userId, HttpUtility.UrlEncode(AchievementTypes.TrainingGuide));

                if (total == valid)
                {
                    item.FlagType = CmdsFlagType.Green;
                    item.FlagTooltip = "Completed";
                }
                else
                {
                    item.FlagType = CmdsFlagType.Red;
                    item.FlagTooltip = "Not Completed";
                }
            }
            else
            {
                item.ProgressText = "0 out of 0 completed";
            }

            list.Add(item);
        }

        private void AddProcedures(Guid userId, Guid organizationId, string organizationCode, List<SummaryDataItem> list)
        {
            var total = 0;
            var valid = 0;
            var table = VCmdsCredentialSearch.SelectPolicySignedGroups(userId, organizationId, false, true, new[] { AchievementTypes.SiteSpecificOperatingProcedure });
            VCmdsCredentialSearch.PolicySignedGroup lastItem = null;

            foreach (var row in table)
            {
                total += row.Total;
                valid += row.Signed;
                lastItem = row;
            }

            var item = new SummaryDataItem
            {
                Title = AchievementTypes.Pluralize(AchievementTypes.SiteSpecificOperatingProcedure, organizationCode),
            };

            if (lastItem != null)
            {
                item.Completed = valid;
                item.Total = total;
                item.ProgressPercent = string.Format("{0:n1}%", CalculateScore(valid, total));
                item.ProgressText = string.Format("{0} out of {1} completed", valid, total);
                item.ProgressUrl = string.Format("/ui/portal/learning/plan?userID={0}&achievementType={1}",
                    userId, HttpUtility.UrlEncode(AchievementTypes.SiteSpecificOperatingProcedure));

                if (total == valid)
                {
                    item.FlagType = CmdsFlagType.Green;
                    item.FlagTooltip = "Completed";
                }
                else
                {
                    item.FlagType = CmdsFlagType.Red;
                    item.FlagTooltip = "Not Completed";
                }
            }
            else
            {
                item.ProgressText = "0 out of 0 completed";
            }

            list.Add(item);
        }

        private static decimal CalculateScore(int a, int b)
        {
            var score =
                b != 0
                    ? 100M * a / b
                    : 100M;

            return Math.Round(score, 1);
        }
    }
}