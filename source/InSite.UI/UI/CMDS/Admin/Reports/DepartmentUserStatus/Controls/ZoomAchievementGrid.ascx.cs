using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Persistence.Plugin.CMDS;

namespace InSite.Custom.CMDS.Reports.Controls
{
    public partial class ZoomAchievementGrid : BaseUserControl
    {
        private class DataItem
        {
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementTitle { get; set; }
            public int CountCP { get; set; }
            public int CountEX { get; set; }
            public int CountNA { get; set; }
            public int CountNC { get; set; }
            public int CountNT { get; set; }
            public int CountRQ { get; set; }
            public int CountSA { get; set; }
            public int CountSV { get; set; }
            public int CountVA { get; set; }

            public void Add(UserStatusAchievement data)
            {
                CountCP += data.CountCP;
                CountEX += data.CountEX;
                CountNC += data.CountNC;

                CountNA += data.CountNA;
                CountNT += data.CountNT;
                CountSA += data.CountSA;

                CountSV += data.CountSV;
                CountVA += data.CountVA;
                CountRQ += data.CountRQ;
            }
        }

        public void LoadData(TUserStatus statistic, IEnumerable<string> visibleColumns)
        {
            var dataSource = new List<DataItem>();

            DataItem currentItem = null;
            DataItem totalItem = new DataItem { AchievementTitle = "Total" };

            var details = TUserStatusSearch.SelectUserStatusAchievement(statistic.OrganizationIdentifier, statistic.DepartmentIdentifier, statistic.UserIdentifier);
            foreach (var detail in details.Where(x => x.AchievementType == statistic.ItemName).OrderBy(x => x.AchievementIdentifier))
            {
                if (currentItem == null || currentItem.AchievementIdentifier != detail.AchievementIdentifier)
                {
                    dataSource.Add(currentItem = new DataItem
                    {
                        AchievementIdentifier = detail.AchievementIdentifier,
                        AchievementTitle = detail.AchievementTitle
                    });
                }

                currentItem.Add(detail);
                totalItem.Add(detail);
            }

            var visibleColumnsIndex = visibleColumns != null
                ? new HashSet<string>(visibleColumns, StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>();

            foreach (DataControlField column in Grid.Columns)
            {
                if (!string.IsNullOrEmpty(column.HeaderText))
                    column.Visible = visibleColumnsIndex.Count == 0 || visibleColumnsIndex.Contains(column.HeaderText);
            }

            Grid.DataSource = dataSource.OrderBy(x => x.AchievementTitle).Append(totalItem);
            Grid.DataBind();
        }

        protected static string NumberOrIcon(object obj, bool isTotal)
        {
            var intVal = (int)obj;

            return intVal == 0
                ? string.Empty
                : intVal == 1 && !isTotal
                    ? "<i class='far fa-check'></i>"
                    : intVal.ToString("n0");
        }

        protected static string NumberOrEmpty(object obj)
        {
            var intVal = (int)obj;

            return intVal == 0
                ? string.Empty
                : intVal.ToString("n0");
        }
    }
}