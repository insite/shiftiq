using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using InSite.Application.Records.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class GradebookReport
    {
        public string Build(IRecordSearch search, Guid gradebook)
        {
            var items = search.GetGradeItemHierarchies(gradebook);
            var users = search.GetEnrollments(new QEnrollmentFilter { GradebookIdentifier = gradebook });
            var table = CreateGradebookTable(users);

            LoadProgressRows(table, search, gradebook, items);
            InsertUserRows(table, users);

            return CsvHelper.ConvertTableToCsv(table, null);
        }

        private DataTable CreateGradebookTable(List<QEnrollment> users)
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("Code"));
            table.Columns.Add(new DataColumn("Name"));
            table.Columns.Add(new DataColumn("Type"));
            table.Columns.Add(new DataColumn("Format"));
            table.Columns.Add(new DataColumn("User"));

            foreach (var user in users)
                table.Columns.Add(new DataColumn(user.LearnerIdentifier.ToString()));

            return table;
        }

        private void LoadProgressRows(DataTable table, IRecordSearch search, Guid gradebook, List<VGradeItemHierarchy> items)
        {
            foreach (var item in items)
            {
                var row = table.NewRow();
                row["Code"] = item.PathCode;
                row["Name"] = new string(' ', item.PathDepth * 4) + item.GradeItemName;
                row["Type"] = item.GradeItemType;
                row["Format"] = item.GradeItemFormat;

                var scores = search.GetGradebookScores(new QProgressFilter { GradebookIdentifier = gradebook, GradeItemIdentifier = item.GradeItemIdentifier });
                foreach (var score in scores)
                    row[score.UserIdentifier.ToString()] = GetProgressValue(item, score);

                table.Rows.Add(row);
            }
        }

        private void InsertUserRows(DataTable table, List<QEnrollment> users)
        {
            InsertUserRow(table, users, 0, "Name", x => x.Learner.UserFullName);
            InsertUserRow(table, users, 1, "Email", x => x.Learner.UserEmail);

            var identifiers = users.Select(x => x.LearnerIdentifier).ToList();
            var values = TPersonFieldSearch.Bind(x => x, x => identifiers.Any(y => y == x.UserIdentifier)).ToList();
            var names = values.Select(x => x.FieldName).Distinct().OrderBy(x => x).ToList();
            for (int i = 0; i < names.Count; i++)
            {
                var name = names[i];
                InsertUserRow(table, users, i + 2, name, x =>
                {
                    return values.FirstOrDefault(v => v.UserIdentifier == x.LearnerIdentifier && v.FieldName == name)?.FieldValue;
                });
            }
        }

        private void InsertUserRow(DataTable table, List<QEnrollment> students, int index, string property, Func<QEnrollment, string> f)
        {
            var row = table.NewRow();
            row["User"] = property + ":";
            foreach (var student in students)
                row[student.LearnerIdentifier.ToString()] = $"{f(student)}";
            table.Rows.InsertAt(row, index);
        }

        private string GetProgressValue(VGradeItemHierarchy item, QProgress score)
        {
            if (item.GradeItemType == "Calculation")
                return $"{score.ProgressPercent:p0}";

            if (item.GradeItemType == "Category")
                return $"{score.ProgressPercent:p0}";

            if (item.GradeItemFormat == "Percent")
                return $"{score.ProgressPercent:p0}";

            if (item.GradeItemFormat == "Boolean")
                return score.ProgressIsCompleted ? $"Completed {score.ProgressCompleted:MMM d, yyyy}" : score.ProgressText;

            if (item.GradeItemFormat == "Text")
                return $"{score.ProgressText}";

            return String.Empty;
        }


    }
}
