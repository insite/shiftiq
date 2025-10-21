using System.Collections.Generic;

using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Scores.Reports
{
    public static class LowestScoreStudentReport
    {
        public static byte[] GetXlsx(QProgressFilter filter)
        {
            var students = ServiceLocator.RecordSearch.GetLowestScoreStudents(filter);

            var sheet = CreateWorksheet(students);

            return XlsxWorksheet.GetBytes(sheet);
        }

        private static XlsxWorksheet CreateWorksheet(List<LowestScoreStudent> students)
        {
            var headerStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var style = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var pointsFormat = "#,##0.00";

            var sheet = new XlsxWorksheet("Course Summary");

            sheet.Columns[0].Width = 30; // Course
            sheet.Columns[1].Width = 30; // Score Item
            sheet.Columns[2].Width = 30; // Student
            sheet.Columns[3].Width = 15; // Points

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Course", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Score Item", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Student", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "Points", Style = headerRightStyle });

            var rowIndex = 1;

            foreach (var row in students)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.GradebookTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.ScoreItemName, Style = style });
                sheet.Cells.Add(new XlsxCell(2, rowIndex) { Value = row.UserFullName, Style = style });
                sheet.Cells.Add(new XlsxCell(3, rowIndex) { Value = row.Points, Style = rightStyle, Format = pointsFormat });

                rowIndex++;
            }

            return sheet;
        }
    }
}