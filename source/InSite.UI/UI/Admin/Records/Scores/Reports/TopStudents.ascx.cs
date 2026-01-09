using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Records.Scores.Reports;
using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Scores
{
    public partial class TopStudents : UserControl
    {
        private class Student
        {
            public string StudentFullName { get; set; }
            public string EmployerName { get; set; }

            public string ScoreValue { get; set; }
        }

        private class ScoreItem
        {
            public string ScoreItemName { get; set; }
            public List<Student> Students { get; set; }
        }
        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<ScoreItem> ScoreItems { get; set; }
        }

        private class RegionItem
        {
            public string EmployerRegion { get; set; }
            public List<AchievementItem> Achievements { get; set; }
        }

        public void LoadReport(QProgressFilter filter)
        {
            PageTitle.InnerText = "Top 10 Students";

            var criteriaItems = GradebookScoreCriteriaHelper.GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            RegionRepeater.ItemDataBound += RegionRepeater_ItemDataBound;
            RegionRepeater.DataSource = CreateData(filter);
            RegionRepeater.DataBind();
        }

        public static byte[] GetXlsx(QProgressFilter filter)
        {
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

            var data = CreateData(filter);

            var sheets = new List<XlsxWorksheet>();

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };

            foreach (var region in data)
            {
                var sheet = new XlsxWorksheet(region.EmployerRegion);

                sheet.Columns[0].Width = 40;
                sheet.Columns[1].Width = 40;
                sheet.Columns[2].Width = 15;

                sheet.Rows[0].Height = criteriaRowHeight;
                sheet.Cells.Add(new XlsxCell(0, 0, 3) { Value = criterias.ToString(), Style = normalStyle });
                sheet.Cells.Add(new XlsxCell(0, 1, 3) { Value = $"{region.EmployerRegion} Employer's Region", Style = boldStyle });

                int row = 3;

                foreach (var achievement in region.Achievements)
                {
                    sheet.Cells.Add(new XlsxCell(0, ++row, 3) { Value = $"Achievement: {achievement.AchievementTitle}", Style = boldCenterStyle });

                    foreach (var scoreItem in achievement.ScoreItems)
                    {
                        XlsxCellRichText scoreHeaderCell = new XlsxCellRichText(0, ++row, 3) { Style = normalStyle };
                        scoreHeaderCell.AddText("Score Item: ", false);
                        scoreHeaderCell.AddText(scoreItem.ScoreItemName, true);
                        sheet.Cells.Add(scoreHeaderCell);
                        sheet.Cells.Add(new XlsxCell(0, ++row) { Value = "Student", Style = boldStyle });
                        sheet.Cells.Add(new XlsxCell(1, row) { Value = "Employer", Style = boldStyle });
                        sheet.Cells.Add(new XlsxCell(2, row) { Value = "Score", Style = boldStyle });

                        foreach (var student in scoreItem.Students)
                        {
                            sheet.Cells.Add(new XlsxCell(0, ++row) { Value = student.StudentFullName, Style = normalStyle });
                            sheet.Cells.Add(new XlsxCell(1, row) { Value = student.EmployerName, Style = normalStyle });
                            sheet.Cells.Add(new XlsxCell(2, row) { Value = student.ScoreValue, Style = normalStyle });
                        }

                        row++;
                    }

                    row++;
                }

                sheets.Add(sheet);
            }

            if (sheets.Count == 0)
                sheets.Add(new XlsxWorksheet("No Data"));

            return XlsxWorksheet.GetBytes(sheets.ToArray());
        }

        private void RegionRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (RegionItem)e.Item.DataItem;

            var achievementRepeater = (Repeater)e.Item.FindControl("AchievementRepeater");
            achievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            achievementRepeater.DataSource = item.Achievements;
            achievementRepeater.DataBind();
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (AchievementItem)e.Item.DataItem;

            var scoreItemRepeater = (Repeater)e.Item.FindControl("ScoreItemRepeater");
            scoreItemRepeater.ItemDataBound += ScoreItemRepeater_ItemDataBound;
            scoreItemRepeater.DataSource = item.ScoreItems;
            scoreItemRepeater.DataBind();
        }

        private void ScoreItemRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (ScoreItem)e.Item.DataItem;

            var studentRepeater = (Repeater)e.Item.FindControl("StudentRepeater");
            studentRepeater.DataSource = item.Students;
            studentRepeater.DataBind();
        }

        private static List<RegionItem> CreateData(QProgressFilter filter)
        {
            var summaries = ServiceLocator.RecordSearch.GetTopStudentSummaries(filter);

            return summaries
                .GroupBy(x => x.EmployerRegion)
                .Select(a => new RegionItem
                {
                    EmployerRegion = a.Key,
                    Achievements = a.GroupBy(x => x.AchievementTitle).Select(b => new AchievementItem
                    {
                        AchievementTitle = b.Key,
                        ScoreItems = b.Select(c => new ScoreItem
                        {
                            ScoreItemName = c.ScoreItemName,
                            Students = c.Students.OrderByDescending(x => x.Percent).Select(d => new Student
                            {
                                EmployerName = d.EmployerName,
                                StudentFullName = d.FullName,
                                ScoreValue = $"{d.Percent:p3}"
                            })
                            .ToList()
                        })
                        .OrderBy(x => x.ScoreItemName)
                        .ToList()
                    })
                    .OrderBy(x => x.AchievementTitle)
                    .ToList()
                }).ToList();
        }
    }
}