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
    public partial class PassingRate : UserControl
    {
        private class ScoreItem
        {
            public string ScoreItemName { get; set; }
            
            public int StudentsTotal { get; set; }

            public int StudentsAbove70 { get; set; }

            public decimal PassingRate => (decimal)StudentsAbove70 / (decimal)StudentsTotal;
        }

        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<ScoreItem> ScoreItems { get; set; }
        }

        public void LoadReport(QProgressFilter filter)
        {
            PageTitle.InnerText = "Students Passing Grade";

            var criteriaItems = GradebookScoreCriteriaHelper.GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            var summaries = ServiceLocator.RecordSearch.GetPassingRateSummaries(filter);

            var data = summaries
                .GroupBy(x => x.AchievementTitle)
                .Select(a => new AchievementItem
                {
                    AchievementTitle = a.Key,
                    ScoreItems = a.Select(b => new ScoreItem
                    {
                        ScoreItemName = b.ScoreItemName,
                        StudentsTotal = b.StudentCount,
                        StudentsAbove70 = b.StudentAbove70Count
                    })
                    .OrderBy(x => x.ScoreItemName)
                    .ToList()
                })
                .OrderBy(x => x.AchievementTitle)
                .ToList();

            AchievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            AchievementRepeater.DataSource = data;
            AchievementRepeater.DataBind();
        }

        public static byte[] GetXlsx(QProgressFilter filter)
        {
            var summaries = ServiceLocator.RecordSearch.GetPassingRateSummaries(filter);

            var data = summaries
                .GroupBy(x => x.AchievementTitle)
                .Select(a => new AchievementItem
                {
                    AchievementTitle = a.Key,
                    ScoreItems = a.Select(b => new ScoreItem
                    {
                        ScoreItemName = b.ScoreItemName,
                        StudentsTotal = b.StudentCount,
                        StudentsAbove70 = b.StudentAbove70Count
                    })
                    .OrderBy(x => x.ScoreItemName)
                    .ToList()
                })
                .OrderBy(x => x.AchievementTitle)
                .ToList();

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

            var sheet = new XlsxWorksheet("Students Passing Grade");

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            var normalCenterStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };

            sheet.Columns[0].Width = 40;
            sheet.Columns[1].Width = 15;
            sheet.Columns[2].Width = 25;
            sheet.Columns[3].Width = 15;

            sheet.Rows[0].Height = criteriaRowHeight;
            sheet.Cells.Add(new XlsxCell(0, 0, 4) { Value = criterias.ToString(), Style = normalStyle });

            int row = 0;

            foreach (var category in data)
            {
                sheet.Cells.Add(new XlsxCell(0, ++row, 4) { Value = category.AchievementTitle, Style = boldCenterStyle });
                sheet.Cells.Add(new XlsxCell(0, ++row) { Value = "Item Name", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = "Students", Style = boldCenterStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = "70% AND above", Style = boldCenterStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = "Passing Rate", Style = boldCenterStyle });

                foreach (var scoreItem in category.ScoreItems)
                {
                    sheet.Cells.Add(new XlsxCell(0, ++row) { Value = scoreItem.ScoreItemName, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = scoreItem.StudentsTotal, Style = normalCenterStyle });
                    sheet.Cells.Add(new XlsxCell(2, row) { Value = scoreItem.StudentsAbove70, Style = normalCenterStyle });
                    sheet.Cells.Add(new XlsxCell(3, row) { Value = string.Format("{0:P2}", scoreItem.PassingRate), Style = normalCenterStyle });
                }

                row++;
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var item = (AchievementItem)e.Item.DataItem;

            var scoreItemRepeater = (Repeater)e.Item.FindControl("ScoreItemRepeater");
            scoreItemRepeater.DataSource = item.ScoreItems;
            scoreItemRepeater.DataBind();
        }
    }
}