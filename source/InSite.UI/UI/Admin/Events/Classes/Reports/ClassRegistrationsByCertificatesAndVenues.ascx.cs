using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

using InSite.Application.Events.Read;
using InSite.UI.Admin.Events.Classes.Reports;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Events.Classes.Reports
{
    public partial class ClassRegistrationsByCertificatesAndVenues : BaseReportControl
    {
        private class ClassItem
        {
            public int ClassNumber { get; set; }
            public string ClassTitle { get; set; }
            public int RegistrationCount { get; set; }
            public string Venue { get; set; }
        }

        private class AchievementItem
        {
            public string AchievementTitle { get; set; }
            public List<ClassItem> Classes { get; set; }
            public int RegistrationCount => Classes.Sum(x => x.RegistrationCount);
        }

        public override string ReportTitle => "Class Registrations by Certificates and Venues";

        public override string ReportFileName => "ClassRegistrationsByCertificatesAndVenues";

        public override byte[] GetPdf(QEventFilter filter)
        {
            PageTitle.InnerText = ReportTitle;

            var achievements = GetAchievements(filter);

            AchievementRepeater.ItemDataBound += AchievementRepeater_ItemDataBound;
            AchievementRepeater.DataSource = achievements;
            AchievementRepeater.DataBind();

            var criteriaItems = GetCriteriaItems(filter);

            SearchCriteriaRepeater.Visible = criteriaItems.Count > 0;
            SearchCriteriaRepeater.DataSource = criteriaItems;
            SearchCriteriaRepeater.DataBind();

            NoCriteriaPanel.Visible = criteriaItems.Count == 0;

            return BuildPdf(PageOrientationType.Portrait, 1400, 980, ReportTitle);
        }

        public override byte[] GetXlsx(QEventFilter filter)
        {
            var achievements = GetAchievements(filter);
            var criteriaItems = GetCriteriaItems(filter);

            var criteriaRowHeight = (criteriaItems.Count + 2) * 15 > 45 ? (criteriaItems.Count + 2) * 15 : 45;
            var criterias = new StringBuilder();
            criterias.AppendLine("Search Criteria");

            if (criteriaItems.Count > 0)
            {
                foreach (var item in criteriaItems)
                    criterias.AppendLine($"{item.Name} = {item.Value}");
            }
            else
                criterias.AppendLine("None");

            var sheet = new XlsxWorksheet(ReportTitle);

            var boldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Left };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
            var boldRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var normalStyle = new XlsxCellStyle { Align = HorizontalAlignment.Left };
            var centerStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };

            sheet.Columns[0].Width = 10;
            sheet.Columns[1].Width = 60;
            sheet.Columns[2].Width = 15;
            sheet.Columns[3].Width = 40;

            sheet.Rows[0].Height = criteriaRowHeight;
            XlsxCellRichText headerCell = new XlsxCellRichText(0, 0, 4) { Style = normalStyle };
            headerCell.AddText(ReportTitle + "\n", true);
            headerCell.AddText(criterias.ToString(), false);
            sheet.Cells.Add(headerCell);

            var row = 1;

            foreach (var achievement in achievements)
            {
                sheet.Cells.Add(new XlsxCell(0, ++row, 4) { Value = achievement.AchievementTitle, Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(1, ++row) { Value = "Class", Style = boldStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = "Students", Style = boldCenterStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = "Venue", Style = boldStyle });

                foreach (var classItem in achievement.Classes)
                {
                    sheet.Cells.Add(new XlsxCell(0, ++row) { Value = classItem.ClassNumber, Style = centerStyle });
                    sheet.Cells.Add(new XlsxCell(1, row) { Value = classItem.ClassTitle, Style = normalStyle });
                    sheet.Cells.Add(new XlsxCell(2, row) { Value = classItem.RegistrationCount, Style = centerStyle });
                    sheet.Cells.Add(new XlsxCell(3, row) { Value = classItem.Venue, Style = normalStyle });
                }

                sheet.Cells.Add(new XlsxCell(1, ++row) { Value = "TOTAL", Style = boldRightStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = achievement.RegistrationCount, Style = boldCenterStyle });

                row += 2;
            }

            return XlsxWorksheet.GetBytes(sheet);
        }

        private List<AchievementItem> GetAchievements(QEventFilter filter)
        {
            var data = ServiceLocator.EventSearch.GetRegistrationCertificateSummary(filter);

            return data
                .GroupBy(x => x.AchievementIdentifier)
                .Select(x =>
                {
                    var achievementTitle = x.First().AchievementTitle;
                    var number = 1;

                    return new AchievementItem
                    {
                        AchievementTitle = achievementTitle,
                        Classes = x
                            .OrderBy(y => y.EventTitle)
                            .Select(y =>
                            {
                                return new ClassItem
                                {
                                    ClassNumber = number++,
                                    ClassTitle = y.EventTitle,
                                    Venue = y.VenueLocationName,
                                    RegistrationCount = y.RegistrationCount
                                };
                            })
                            .ToList()
                    };
                }).ToList();
        }

        private void AchievementRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var achievement = (AchievementItem)e.Item.DataItem;

            var registrationRepeater = (Repeater)e.Item.FindControl("ClassRepeater");
            registrationRepeater.DataSource = achievement.Classes;
            registrationRepeater.DataBind();

            var venues = achievement.Classes
                .GroupBy(x => x.Venue)
                .Select(x => new
                {
                    Venue = x.Key ?? "N/A",
                    RegistrationCount = x.Sum(y => y.RegistrationCount)
                })
                .OrderBy(x => x.Venue)
                .ToList();

            var venueRepeater = (Repeater)e.Item.FindControl("VenueRepeater");
            venueRepeater.DataSource = venues;
            venueRepeater.DataBind();
        }
    }
}