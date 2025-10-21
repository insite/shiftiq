using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Persistence.Plugin.CMDS;
using InSite.UI.CMDS.Common.Models;

using Shift.Constant;

namespace InSite.UI.Portal.Home.Controls
{
    public partial class CmdsOrientationSummary : UserControl
    {
        public IEnumerable<SummaryDataItem> DataItems { get; private set; }

        public void BindModelToControls(Guid userId, Guid organizationId)
        {
            var items = new List<SummaryDataItem>();

            AddOrientations(userId, organizationId, items);

            Repeater.DataSource = DataItems = items;
            Repeater.DataBind();
        }

        private void AddOrientations(Guid user, Guid organization, List<SummaryDataItem> list)
        {
            {
                VCmdsCredentialSearch.CountOrientations(user, organization, true, out int valid, out int total);

                var item = new SummaryDataItem
                {
                    Title = "Mandatory Orientations",
                    ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(valid, total)) : "-",
                    ProgressText = string.Format("{0} out of {1} completed", valid, total),
                };

                if (total == valid)
                {
                    item.FlagType = CmdsFlagType.Green;
                    item.FlagTooltip = "Valid";
                }
                else
                {
                    item.FlagType = CmdsFlagType.Red;
                    item.FlagTooltip = "Pending or Expired";
                }

                list.Add(item);
            }

            {
                VCmdsCredentialSearch.CountOrientations(user, organization, false, out int valid, out int total);

                var item = new SummaryDataItem
                {
                    Title = "Optional Orientations",
                    ProgressPercent = total > 0 ? string.Format("{0:n1}%", CalculateScore(valid, total)) : "-",
                    ProgressText = string.Format("{0} out of {1} completed", valid, total),
                };

                if (total == valid)
                {
                    item.FlagType = CmdsFlagType.Green;
                    item.FlagTooltip = "Valid";
                }
                else
                {
                    item.FlagType = CmdsFlagType.Red;
                    item.FlagTooltip = "Pending or Expired";
                }

                list.Add(item);
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
    }
}