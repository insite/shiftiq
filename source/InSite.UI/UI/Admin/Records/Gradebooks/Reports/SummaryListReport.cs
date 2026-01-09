using System;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Records.Gradebooks.Reports
{
    public static class SummaryListReport
    {
        public static byte[] GetXlsx(Guid gradebookIdentifier)
        {
            var headerLeftStyle = new XlsxCellStyle { IsBold = true };
            var headerRightStyle = new XlsxCellStyle { IsBold = true, Align = HorizontalAlignment.Right };

            var sheet = new XlsxWorksheet("Summary List");
            sheet.Columns[0].Width = 20; // Code
            sheet.Columns[1].Width = 30; // Student
            sheet.Columns[2].Width = 20; // Final Score

            var codeCaption = CurrentSessionState.Identity.Organization.OrganizationIdentifier == OrganizationIdentifiers.RCABC
                ? "Tradeworker #"
                : "Person Code";

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = codeCaption, Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Student", Style = headerLeftStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Final Score", Style = headerRightStyle });

            AddData(gradebookIdentifier, sheet);

            return XlsxWorksheet.GetBytes(sheet);
        }

        private static void AddData(Guid gradebookIdentifier, XlsxWorksheet sheet)
        {
            var leftStyle = new XlsxCellStyle { };
            var rightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
            var scoreFormat = "#0.00%";

            var gradeItems = ServiceLocator.RecordSearch.GetGradeItems(gradebookIdentifier);
            var topItem = gradeItems
                .Where(x => x.ParentGradeItemIdentifier == null)
                .OrderBy(x => x.GradeItemSequence)
                .FirstOrDefault();

            var progresses = topItem != null
                ? ServiceLocator.RecordSearch.GetGradebookScores(
                    new QProgressFilter { GradeItemIdentifier = topItem.GradeItemIdentifier },
                    null,
                    null,
                    x => x.Learner.Persons
                    )
                : null;

            if (progresses == null)
                return;

            var students = progresses
                .Select(x =>
                {
                    var person = x.Learner.Persons.FirstOrDefault(y => y.OrganizationIdentifier == CurrentSessionState.Identity.Organization.OrganizationIdentifier);

                    return new
                    {
                        Code = person?.PersonCode,
                        Student = x.Learner.UserFullName,
                        FinalScore = x.ProgressPercent
                    };
                })
                .ToList();

            for (int i = 0; i < students.Count; i++)
            {
                var student = students[i];
                var row = i + 1;

                sheet.Cells.Add(new XlsxCell(0, row) { Value = student.Code, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = student.Student, Style = leftStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = student.FinalScore, Style = leftStyle, Format = scoreFormat });
            }
        }
    }
}