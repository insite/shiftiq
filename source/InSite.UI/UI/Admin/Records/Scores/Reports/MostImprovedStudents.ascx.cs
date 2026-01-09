using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Scores.Reports;
using InSite.Application.Records.Read;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Scores
{
    public partial class MostImprovedStudents : UserControl
    {
        public void LoadReport(QProgressFilter filter)
        {
            PageTitle.InnerText = "Top 10 Most Improved Students";

            var data = ServiceLocator.RecordSearch.GetMostImprovedStudents(filter);

            TradeRepeater.ItemDataBound += TradeRepeater_ItemDataBound;
            TradeRepeater.DataSource = data;
            TradeRepeater.DataBind();

            var criteriaItems = GradebookScoreCriteriaHelper.GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;
        }

        public static byte[] GetXlsx(QProgressFilter filter)
        {
            var data = ServiceLocator.RecordSearch.GetMostImprovedStudents(filter);

            var criteriaItems = GradebookScoreCriteriaHelper.GetCriteriaItems(filter);

            int criteriaRowHeight = (criteriaItems.Count + 1) * 15 > 30 ? (criteriaItems.Count + 1) * 15 : 30;
            StringBuilder criterias = new StringBuilder();
            criterias.AppendLine("Search Criteria");

            if (criteriaItems.Count > 0)
            {
                foreach (var item in criteriaItems)
                    criterias.AppendLine($"{item.Name} = {item.Value}");
            }
            else
                criterias.AppendLine("None");

            var sheets = new List<XlsxWorksheet>();

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };

            foreach (var category in data)
            {
                var sheet = new XlsxWorksheet(category.AchievementDescription);

                sheet.Columns[0].Width = 40;
                sheet.Columns[1].Width = 40;
                sheet.Columns[2].Width = 15;
                sheet.Columns[3].Width = 15;
                sheet.Columns[4].Width = 30;
                sheet.Columns[5].Width = 15;

                sheet.Rows[0].Height = criteriaRowHeight;
                sheet.Cells.Add(new XlsxCell(0, 0, 6) { Value = criterias.ToString(), Style = normalStyle });

                sheet.Rows[1].Height = 30;
                XlsxCellRichText categoryHeader = new XlsxCellRichText(0, 1, 6) { Style = boldStyle };
                categoryHeader.AddText(category.AchievementDescription + "\n", true);
                categoryHeader.AddText("Out of ", false);
                categoryHeader.AddText(category.TotalStudentCount.ToString(), true);
                categoryHeader.AddText(" students");
                sheet.Cells.Add(categoryHeader);

                sheet.Cells.Add(new XlsxCell(0, 2) { Value = "Student", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(1, 2) { Value = "Employer", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(2, 2) { Value = "Started", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(3, 2) { Value = "Completed", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(4, 2) { Value = "Grades", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(5, 2) { Value = "Difference", Style = boldStyle });

                for (int i = 0; i < category.Students.Count; i++)
                {
                    var student = category.Students[i];
                    sheet.Rows[i + 3].Height = student.Levels.Count * 15;
                    sheet.Cells.Add(new XlsxCell(0, i + 3) { Value = $"{student.UserFullName}\n{student.UserEmail}", Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(1, i + 3) { Value = $"{student.EmployerName}\n{student.EmployerRegion}", Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(2, i + 3) { Value = ((DateTimeOffset?)student.EventScheduledStart).FormatDateOnly("MMMM, yyyy"), Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(3, i + 3) { Value = student.EventScheduledEnd.FormatDateOnly("MMMM, yyyy"), Style = normalStyle });

                    StringBuilder levels = new StringBuilder();
                    foreach (var level in student.Levels)
                        levels.AppendLine(String.Format("{0}: {1:P2}", level.AchievementTitle, level.Percent));

                    sheet.Cells.Add(new XlsxCell(4, i + 3) { Value = levels.ToString(), Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(5, i + 3) { Value = String.Format("{0:P2}", student.Difference), Style = normalStyle });
                }

                sheets.Add(sheet);
            }

            if (sheets.Count == 0)
                sheets.Add(new XlsxWorksheet("No Data"));

            return XlsxWorksheet.GetBytes(sheets.ToArray());
        }

        private void TradeRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var trade = (MostImprovedStudent)e.Item.DataItem;

            var studentRepeater = (Repeater)e.Item.FindControl("StudentRepeater");
            studentRepeater.ItemDataBound += StudentRepeater_ItemDataBound;
            studentRepeater.DataSource = trade.Students;
            studentRepeater.DataBind();
        }


        private void StudentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var student = (MostImprovedStudent.Student)e.Item.DataItem;

            var levelRepeater = (Repeater)e.Item.FindControl("LevelRepeater");
            levelRepeater.DataSource = student.Levels;
            levelRepeater.DataBind();
        }

        protected static string GetLocalDate(object s)
        {
            if (s == null)
                return "N/A";

            var timeZone = CurrentSessionState.Identity.User.TimeZone;

            return ((DateTimeOffset)s).FormatDateOnly(timeZone);
        }
    }
}