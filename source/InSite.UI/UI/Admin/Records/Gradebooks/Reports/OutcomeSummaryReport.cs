using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Gradebooks.Reports
{
    public static class OutcomeSummaryReport
    {
        private static System.Drawing.Color FontColor = System.Drawing.Color.FromArgb(60, 122, 184);
        private static System.Drawing.Color DarkColor = System.Drawing.Color.FromArgb(189, 215, 238);
        private static System.Drawing.Color LightColor = System.Drawing.Color.FromArgb(221, 235, 247);

        private class GradebookRow
        {
            public string GradebookTitle { get; set; }
            public List<(Guid StandardIdentifier, decimal Score)> Scores { get; set; }
            public List<(Guid StandardIdentifier, int Count)> Students { get; set; }
        }

        public static byte[] GetXlsx(QGradebookFilter filter)
        {
            var courseOutcomeSummary = ServiceLocator.RecordSearch.GetCourseOutcomeSummary(filter);
            var outcomeSummary = ServiceLocator.RecordSearch.GetOutcomeSummary(filter);

            var outcomes = courseOutcomeSummary
                .Select(x => (x.StandardIdentifier, x.StandardTitle))
                .Distinct()
                .OrderBy(x => x.StandardTitle)
                .ToList();

            var gradebooks = courseOutcomeSummary
                .GroupBy(x => new { x.GradebookIdentifier, x.GradebookTitle, x.Reference, x.Term })
                .Select(x => new GradebookRow
                {
                    GradebookTitle = x.Key.GradebookTitle,
                    Scores = x.Select(y => (y.StandardIdentifier, y.AvgScore)).ToList(),
                    Students = x.Select(y => (y.StandardIdentifier, y.StudentCount)).ToList()
                })
                .ToList();

            var scoreSheet = CreateScoreWorksheet(outcomes, gradebooks);
            var studentSheet = CreateStudentWorksheet(outcomes, gradebooks);
            var courseSummarySheet = CreateCourseSummary(courseOutcomeSummary);
            var summarySheet = CreateSummary(outcomeSummary);

            return XlsxWorksheet.GetBytes(scoreSheet, studentSheet, courseSummarySheet, summarySheet);
        }

        private static XlsxWorksheet CreateScoreWorksheet(List<(Guid, string Title)> outcomes, List<GradebookRow> gradebooks)
        {
            var headerStyle = new XlsxCellStyle { IsBold = true, VAlign = XlsxCellVAlign.Center };
            var boldStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };

            var sheet = new XlsxWorksheet("SUMMARY - Avg Scores");
            
            sheet.Columns[0].Width = 30; // GradebookTitle
            
            for (int i = 1; i <= outcomes.Count; i++)
                sheet.Columns[i].Width = 20;

            sheet.Columns[outcomes.Count + 1].Width = 20;

            sheet.Rows[0].Height = 30;
            sheet.Rows[1].Height = 100;

            sheet.Cells.Add(new XlsxCell(0, 0, outcomes.Count + 2) { Value = "Average Student Score per Outcome per Course", Style = headerStyle });

            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "Courses", Style = boldStyle });

            for (int i = 1; i <= outcomes.Count; i++)
                sheet.Cells.Add(new XlsxCell(i, 1) { Value = outcomes[i - 1].Title, Style = boldCenterStyle });

            sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, 1) { Value = "Total", Style = boldCenterStyle });

            AddScoreData(outcomes, gradebooks, sheet);

            return sheet;
        }

        private static void AddScoreData(List<(Guid StandardIdentifier, string)> outcomes, List<GradebookRow> gradebooks, XlsxWorksheet sheet)
        {
            var boldStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle1 = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };
            var boldCenterStyle2 = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };
            var centerStyle1 = new XlsxCellStyle { FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center };
            var centerStyle2 = new XlsxCellStyle { FontColor = FontColor, BackgroundColor = DarkColor, Align = HorizontalAlignment.Center };
            var numericFormat = "#,##0.00";
            var row = 2;

            foreach (var gradebook in gradebooks)
            {
                var style = row % 2 == 0 ? centerStyle2 : centerStyle1;
                var gradebookTotalStyle = row % 2 == 0 ? boldCenterStyle2 : boldCenterStyle1;

                sheet.Cells.Add(new XlsxCell(0, row) { Value = gradebook.GradebookTitle, Style = boldStyle });

                for (int k = 0; k < outcomes.Count; k++)
                {
                    var score = gradebook.Scores.Find(x => x.StandardIdentifier == outcomes[k].StandardIdentifier);

                    sheet.Cells.Add(new XlsxCell(k + 1, row) { Value = score.Score, Style = style, Format = numericFormat });
                }

                sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, row)
                {
                    Value = gradebook.Scores.Select(x => x.Score).Average(),
                    Style = gradebookTotalStyle,
                    Format = numericFormat
                });

                row++;
            }

            var totalStyle = row % 2 == 0 ? boldCenterStyle2 : boldCenterStyle1;

            sheet.Cells.Add(new XlsxCell(0, row) { Value = "Total", Style = boldStyle });

            for (int k = 0; k < outcomes.Count; k++)
            {
                var standardIdentifier = outcomes[k].StandardIdentifier;

                sheet.Cells.Add(new XlsxCell(k + 1, row)
                {
                    Value = gradebooks.Average(x => x.Scores.Find(y => y.StandardIdentifier == standardIdentifier).Score),
                    Style = totalStyle,
                    Format = numericFormat
                });
            }

            var totalAverage = gradebooks.Count > 0
                ? gradebooks.Average(x => x.Scores.Select(y => y.Score).Average())
                : 0;

            sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, row) { Value = totalAverage, Style = totalStyle, Format = numericFormat });
        }

        private static XlsxWorksheet CreateStudentWorksheet(List<(Guid, string Title)> outcomes, List<GradebookRow> gradebooks)
        {
            var headerStyle = new XlsxCellStyle { IsBold = true, VAlign = XlsxCellVAlign.Center };
            var boldStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };

            var sheet = new XlsxWorksheet("SUMMARY - Student Counts");

            sheet.Columns[0].Width = 30; // GradebookTitle

            for (int i = 1; i <= outcomes.Count; i++)
                sheet.Columns[i].Width = 20;

            sheet.Columns[outcomes.Count + 1].Width = 20;

            sheet.Rows[0].Height = 30;
            sheet.Rows[1].Height = 100;

            sheet.Cells.Add(new XlsxCell(0, 0, outcomes.Count + 2) { Value = "Number of Students with Outcomes Results per Course", Style = headerStyle });

            sheet.Cells.Add(new XlsxCell(0, 1) { Value = "Courses", Style = boldStyle });

            for (int i = 1; i <= outcomes.Count; i++)
                sheet.Cells.Add(new XlsxCell(i, 1) { Value = outcomes[i - 1].Title, Style = boldCenterStyle });

            sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, 1) { Value = "Total", Style = boldCenterStyle });

            AddStudentData(outcomes, gradebooks, sheet);

            return sheet;
        }

        private static void AddStudentData(List<(Guid StandardIdentifier, string)> outcomes, List<GradebookRow> gradebooks, XlsxWorksheet sheet)
        {
            var boldStyle = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, VAlign = XlsxCellVAlign.Center };
            var boldCenterStyle1 = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };
            var boldCenterStyle2 = new XlsxCellStyle { IsBold = true, FontColor = FontColor, BackgroundColor = DarkColor, Align = HorizontalAlignment.Center, VAlign = XlsxCellVAlign.Center, WrapText = true };
            var centerStyle1 = new XlsxCellStyle { FontColor = FontColor, BackgroundColor = LightColor, Align = HorizontalAlignment.Center };
            var centerStyle2 = new XlsxCellStyle { FontColor = FontColor, BackgroundColor = DarkColor, Align = HorizontalAlignment.Center };
            var row = 2;

            foreach (var gradebook in gradebooks)
            {
                var style = row % 2 == 0 ? centerStyle2 : centerStyle1;
                var gradebookTotalStyle = row % 2 == 0 ? boldCenterStyle2 : boldCenterStyle1;

                sheet.Cells.Add(new XlsxCell(0, row) { Value = gradebook.GradebookTitle, Style = boldStyle });

                for (int k = 0; k < outcomes.Count; k++)
                {
                    var student = gradebook.Students.Find(x => x.StandardIdentifier == outcomes[k].StandardIdentifier);

                    sheet.Cells.Add(new XlsxCell(k + 1, row) { Value = student.Count, Style = style });
                }

                sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, row)
                {
                    Value = gradebook.Students.Sum(x => x.Count),
                    Style = gradebookTotalStyle
                });

                row++;
            }

            var totalStyle = row % 2 == 0 ? boldCenterStyle2 : boldCenterStyle1;

            sheet.Cells.Add(new XlsxCell(0, row) { Value = "Total", Style = boldStyle });

            for (int k = 0; k < outcomes.Count; k++)
            {
                var standardIdentifier = outcomes[k].StandardIdentifier;

                sheet.Cells.Add(new XlsxCell(k + 1, row)
                {
                    Value = gradebooks.Sum(x => x.Students.Find(y => y.StandardIdentifier == standardIdentifier).Count),
                    Style = totalStyle
                });
            }

            sheet.Cells.Add(new XlsxCell(outcomes.Count + 1, row) { Value = gradebooks.Sum(x => x.Students.Sum(y => y.Count)), Style = totalStyle });
        }

        private static XlsxWorksheet CreateCourseSummary(List<CourseOutcomeSummary> rows)
        {
            rows = rows
                .OrderBy(x => x.Term)
                .ThenBy(x => x.GradebookTitle)
                .ThenBy(x => x.StandardTitle)
                .ToList();

            var headerStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var style = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var scoreFormat = "#,##0.00";

            var sheet = new XlsxWorksheet("Course Summary");

            sheet.Columns[0].Width = 14; // Term
            sheet.Columns[1].Width = 14; // Course ID
            sheet.Columns[2].Width = 30; // Course Name
            sheet.Columns[3].Width = 30; // Outcome Group
            sheet.Columns[4].Width = 30; // Outcome
            sheet.Columns[5].Width = 15; // Total Students
            sheet.Columns[6].Width = 15; // Mean Score
            sheet.Columns[7].Width = 15; // Std Score

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Term", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Course ID", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Course", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "Outcome Group", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Outcome", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Total Students", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(6, 0) { Value = "Mean Score", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(7, 0) { Value = "Std Score", Style = headerRightStyle });

            var rowIndex = 1;

            foreach (var row in rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Term, Style = style });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = GetCourseID(row.Reference), Style = style });
                sheet.Cells.Add(new XlsxCell(2, rowIndex) { Value = row.GradebookTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(3, rowIndex) { Value = row.ParentStandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(4, rowIndex) { Value = row.StandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(5, rowIndex) { Value = row.StudentCount, Style = rightStyle });
                sheet.Cells.Add(new XlsxCell(6, rowIndex) { Value = row.AvgScore, Style = rightStyle, Format = scoreFormat });
                sheet.Cells.Add(new XlsxCell(7, rowIndex) { Value = (decimal?)row.StdScore ?? 0, Style = rightStyle, Format = scoreFormat });

                rowIndex++;
            }

            return sheet;
        }

        private static string GetCourseID(string reference)
        {
            return !string.IsNullOrEmpty(reference) && reference.StartsWith("Canvas Course ID = ") && reference.Length > "Canvas Course ID = ".Length
                ? reference.Substring("Canvas Course ID = ".Length)
                : string.Empty;
        }

        private static XlsxWorksheet CreateSummary(List<OutcomeSummary> rows)
        {
            rows = rows
                .OrderBy(x => x.Term)
                .ThenBy(x => x.StandardTitle)
                .ToList();

            var headerStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };
            var style = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var scoreFormat = "#,##0.00";

            var sheet = new XlsxWorksheet("Term Summary");

            sheet.Columns[0].Width = 14; // Term
            sheet.Columns[1].Width = 30; // Outcome Group
            sheet.Columns[2].Width = 30; // Outcome
            sheet.Columns[3].Width = 15; // Total Students
            sheet.Columns[4].Width = 15; // Mean Score
            sheet.Columns[5].Width = 15; // Std Score

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Term", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Outcome Group", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Outcome", Style = headerStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = "Total Students", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Mean Score", Style = headerRightStyle });
            sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Std Score", Style = headerRightStyle });

            var rowIndex = 1;

            foreach (var row in rows)
            {
                sheet.Cells.Add(new XlsxCell(0, rowIndex) { Value = row.Term, Style = style });
                sheet.Cells.Add(new XlsxCell(1, rowIndex) { Value = row.ParentStandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(2, rowIndex) { Value = row.StandardTitle, Style = style });
                sheet.Cells.Add(new XlsxCell(3, rowIndex) { Value = row.StudentCount, Style = rightStyle });
                sheet.Cells.Add(new XlsxCell(4, rowIndex) { Value = row.AvgScore, Style = rightStyle, Format = scoreFormat });
                sheet.Cells.Add(new XlsxCell(5, rowIndex) { Value = (decimal?)row.StdScore ?? 0, Style = rightStyle, Format = scoreFormat });

                rowIndex++;
            }

            return sheet;
        }
    }
}