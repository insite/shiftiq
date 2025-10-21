using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Admin.Records.Reports.Models
{
    public class AcademicYearReport
    {
        private static XlsxCellStyle BoldStyle = new XlsxCellStyle { IsBold = true };
        private static XlsxCellStyle CenteredBoldStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Center };
        private static XlsxCellStyle CenteredStyle = new XlsxCellStyle { Align = HorizontalAlignment.Center };
        private static string ScoreFormat = "#,##0.00";

        public static byte[] GetXlsx(Guid organizationIdentifier, HashSet<Guid> gradebookPeriods)
        {
            var outcomeSummary = ServiceLocator.RecordSearch
                .GetAcademicYearOutcome(organizationIdentifier, gradebookPeriods)
                .OrderBy(x => x.ParentStandardTitle)
                .ThenBy(x => x.StandardTitle)
                .ToList();

            var sheet = CreateSheet(organizationIdentifier, gradebookPeriods);

            AddData(sheet, outcomeSummary, gradebookPeriods.Count + 3);

            return XlsxWorksheet.GetBytes(sheet);
        }

        private static XlsxWorksheet CreateSheet(Guid organizationIdentifier, HashSet<Guid> gradebookPeriods)
        {
            var sheet = new XlsxWorksheet("Academic Year");
            sheet.Columns[0].Width = 40; // Outcome Groups
            sheet.Columns[1].Width = 40; // Outcome
            sheet.Columns[2].Width = 20; // Students
            sheet.Columns[3].Width = 20; // Mean Score
            sheet.Columns[4].Width = 20; // Std Score

            var filter = new QPeriodFilter { OrganizationIdentifier = organizationIdentifier, Identifiers = gradebookPeriods };
            var periods = ServiceLocator.PeriodSearch.GetPeriods(filter);

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Included Periods:", Style = BoldStyle });

            int row = 1;

            foreach (var period in periods)
            {
                sheet.Cells.Add(new XlsxCell(0, row++) { Value = period.PeriodName });
            }

            row++;

            sheet.Cells.Add(new XlsxCell(0, row) { Value = "Outcome Group", Style = CenteredBoldStyle });
            sheet.Cells.Add(new XlsxCell(1, row) { Value = "Outcome", Style = CenteredBoldStyle });
            sheet.Cells.Add(new XlsxCell(2, row) { Value = "Students", Style = CenteredBoldStyle });
            sheet.Cells.Add(new XlsxCell(3, row) { Value = "Mean Score", Style = CenteredBoldStyle });
            sheet.Cells.Add(new XlsxCell(4, row) { Value = "Std Score", Style = CenteredBoldStyle });

            return sheet;
        }

        private static void AddData(XlsxWorksheet sheet, List<AcademicYearOutcome> outcomeSummary, int row)
        {
            foreach (var item in outcomeSummary)
            {
                sheet.Cells.Add(new XlsxCell(0, row) { Value = item.ParentStandardTitle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = item.StandardTitle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = item.StudentCount, Style = CenteredStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = item.AvgScore, Style = CenteredStyle, Format = ScoreFormat });
                sheet.Cells.Add(new XlsxCell(4, row) { Value = item.StdScore, Style = CenteredStyle, Format = ScoreFormat });

                row++;
            }
        }
    }
}