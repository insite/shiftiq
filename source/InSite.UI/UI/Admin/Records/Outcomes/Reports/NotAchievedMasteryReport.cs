using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Outcomes.Reports
{
    public static class NotAchievedMasteryReport
    {
        public static byte[] GetXlsx(QGradebookCompetencyValidationFilter filter)
        {
            filter.NotAchievedMastery = true;

            var validations = ServiceLocator.RecordSearch
                .GetValidations(filter, null, null, x => x.Standard.Parent, x => x.Student)
                .OrderBy(x => x.Standard.Parent.StandardTitle)
                .ThenBy(x => x.Standard.StandardTitle)
                .ThenBy(x => x.Student.UserFullName)
                .ToList();

            var sheet = CreateWorksheet(validations);

            return XlsxWorksheet.GetBytes(sheet);
        }

        private static XlsxWorksheet CreateWorksheet(List<QGradebookCompetencyValidation> validations)
        {
            var headerStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var style = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var pointsFormat = "#,##0.00";

            var sheet = new XlsxWorksheet("Course Summary");

            sheet.Columns[0].Width = 30; // Outcome Group
            sheet.Columns[1].Width = 30; // Outcome
            sheet.Columns[2].Width = 15; // Mastery Points
            sheet.Columns[3].Width = 30; // Student
            sheet.Columns[4].Width = 15; // Points

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Outcome Group", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Outcome", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Mastery Points", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "Student", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Points", Style = headerRightStyle });

            var rowIndex = 1;

            foreach (var row in validations)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Standard.Parent.StandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.Standard.StandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(2, rowIndex) { Value = row.Standard.MasteryPoints, Style = rightStyle, Format = pointsFormat });
                sheet.Cells.Add(new XlsxCell(3, rowIndex) { Value = row.Student.UserFullName, Style = style });
                sheet.Cells.Add(new XlsxCell(4, rowIndex) { Value = row.ValidationPoints, Style = rightStyle, Format = pointsFormat });

                rowIndex++;
            }

            return sheet;
        }
    }
}