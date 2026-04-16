using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Attempts.Read;
using InSite.Common;
using InSite.Domain.Banks;
using InSite.Domain.Organizations;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Toolbox;

using ExportData = InSite.Admin.Assessments.Attempts.Utilities.AttemptReportHelper.ExportData;

namespace InSite.Admin.Assessments.Attempts.Utilities
{
    public static class AttemptReportBySetExport
    {
        #region Classes

        private class ExportAttemptEntity : AttemptAnalysis.AttemptEntity
        {
            public string LearnerFirstName { get; private set; }
            public string LearnerMiddleName { get; private set; }
            public string LearnerLastName { get; private set; }
            public string LearnerUserEmail { get; private set; }
            public string LearnerPersonCode { get; private set; }
            public DateTime? LearnerBirthdate { get; private set; }

            public string FormName { get; private set; }
            public string FormCode { get; private set; }

            public static new readonly Expression<Func<QAttempt, AttemptAnalysis.AttemptEntity>> Binder =
                LinqExtensions1.Expr<QAttempt, AttemptAnalysis.AttemptEntity>(x => new ExportAttemptEntity
                {
                    AttemptIdentifier = x.AttemptIdentifier,
                    FormIdentifier = x.FormIdentifier,
                    AttemptStarted = x.AttemptStarted,
                    AttemptSubmitted = x.AttemptSubmitted,
                    AttemptGraded = x.AttemptGraded,
                    AttemptNumber = x.AttemptNumber,
                    AttemptScore = x.AttemptScore,
                    AttemptPoints = x.AttemptPoints,
                    AttemptIsPassing = x.AttemptIsPassing,

                    LearnerUserIdentifier = x.LearnerUserIdentifier,
                    LearnerFirstName = x.LearnerUser.UserFirstName,
                    LearnerMiddleName = x.LearnerUser.UserMiddleName,
                    LearnerLastName = x.LearnerUser.UserLastName,
                    LearnerUserEmail = x.LearnerUser.UserEmail,
                    LearnerPersonCode = x.LearnerPerson.PersonCode,
                    LearnerBirthdate = x.LearnerPerson.Birthdate,

                    FormName = x.Form.FormName,
                    FormCode = x.Form.FormCode,
                });
        }

        private class ExportQuestionEntity : AttemptReportHelper.ExportQuestionEntity
        {
            public static new readonly Expression<Func<QAttemptQuestion, AttemptAnalysis.QuestionEntity>> Binder = LinqExtensions1.Expr<QAttemptQuestion, AttemptAnalysis.QuestionEntity>(x => new ExportQuestionEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                QuestionIdentifier = x.QuestionIdentifier,
                ParentQuestionIdentifier = x.ParentQuestionIdentifier,
                QuestionSequence = x.QuestionSequence,
                QuestionPoints = x.QuestionPoints,
                AnswerPoints = x.AnswerPoints,
                AnswerOptionKey = x.AnswerOptionKey,

                QuestionType = x.QuestionType,
                AnswerOptionSequence = x.AnswerOptionSequence,
                QuestionText = x.QuestionText,
                AnswerText = x.AnswerText,
                QuestionCalculationMethod = x.QuestionCalculationMethod,
                AnswerFileIdentifier = x.AnswerFileIdentifier,

                CompetencyItemLabel = x.CompetencyItemLabel,
                CompetencyItemCode = x.CompetencyItemCode,
                CompetencyItemTitle = x.CompetencyItemTitle,
                CompetencyItemIdentifier = x.CompetencyItemIdentifier,

                CompetencyAreaLabel = x.CompetencyAreaLabel,
                CompetencyAreaCode = x.CompetencyAreaCode,
                CompetencyAreaTitle = x.CompetencyAreaTitle,
                CompetencyAreaIdentifier = x.CompetencyAreaIdentifier
            });

            public string CompetencyItemValue => GetCompetencyValue(CompetencyItemLabel, CompetencyItemCode, CompetencyItemTitle);
            public string CompetencyAreaValue => GetCompetencyValue(CompetencyItemLabel, CompetencyItemCode, CompetencyItemTitle);

            private static string GetCompetencyValue(string label, string code, string title)
            {
                var prefix = (label + " " + code).Trim();

                if (title.IsEmpty())
                    return prefix;

                if (prefix.IsNotEmpty())
                    return prefix + ". " + title;

                return title;
            }
        }

        private class SetInfo
        {
            public Guid SetId { get; set; }
            public string SetName { get; set; }
            public int SetSequence { get; internal set; }
            public Question[] Questions { get; set; }
        }

        private class ReportData
        {
            public AttemptReportHelper.ExportData ExportData { get; set; }
            public AttemptAnalysis.AttemptEntity[] Attempts { get; set; }
            public SetInfo[] Sets { get; set; }
            public Dictionary<MultiKey<Guid, Guid>, ExportQuestionEntity> Answers { get; set; }
        }

        #endregion

        #region Properties

        private static Domain.Foundations.ISecurityFramework Identity => CurrentSessionState.Identity;
        private static OrganizationState Organization => Identity.Organization;

        #endregion

        public static byte[] GetXlsx(QAttemptFilter filter)
        {
            var data = GetReportData(filter);
            var usedSheetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using (var excel = new ExcelPackage())
            {
                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Calibri";
                defaultStyle.Font.Size = 11;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var headerStyle = excel.Workbook.Styles.CreateNamedStyle("Header");
                headerStyle.Style.Font.Bold = true;

                foreach (var setInfo in data.Sets)
                {
                    var name = GetSheetName(usedSheetNames, setInfo);
                    var sheet = excel.Workbook.Worksheets.Add(name);
                    sheet.Cells.Style.WrapText = true;

                    AddSetInfo(sheet, data, setInfo);
                }

                excel.Workbook.Properties.Title = "Exam Attempt Report";
                excel.Workbook.Properties.Company = Organization.CompanyName;
                excel.Workbook.Properties.Author = Identity.User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                return excel.GetAsByteArray();
            }
        }

        private static readonly char[] InvalidSheetNameChars = new[] { '\\', '/', '*', '[', ']', ':', '?' };

        private static string GetSheetName(HashSet<string> usedSheetNames, SetInfo setInfo)
        {
            var sheetName = string.Concat(setInfo.SetName.EmptyIfNull().Split(InvalidSheetNameChars))
                .Trim().IfNullOrEmpty("Unnamed Set");

            if (!usedSheetNames.Contains(sheetName))
                return sheetName;

            var baseName = sheetName;
            var suffix = 1;

            while (!usedSheetNames.Add(sheetName))
                sheetName = baseName + $" {suffix++:n0}";

            return sheetName;
        }

        private static readonly (string Column, string Title, Type Type, string Format, double? Width, GetQuestionColumnValue GetValue)[] QuestionColumns =
        {
            ("Q{0}ID", "Identifier",              typeof(Guid), null, 40,      (ExportData d, Question bq, ExportQuestionEntity aq) => (object)aq.QuestionIdentifier),
            ("Q{0}CD", "Question Code",           typeof(string), null, null,  (ExportData d, Question bq, ExportQuestionEntity aq) => bq.Classification.Code ?? (object)DBNull.Value),
            ("Q{0}TXT", "Question Text",          typeof(string), null, null,  (ExportData d, Question bq, ExportQuestionEntity aq) => aq.QuestionText ?? (object)DBNull.Value),
            ("Q{0}RSP", "Response Text",          typeof(string), null, null,  GetResponseText),
            ("Q{0}AOS", "Answer Option Sequence", typeof(int), null, null,     GetAnswerOptionSequence),
            ("Q{0}PNT", "Answer Points",          typeof(decimal), null, null, (ExportData d, Question bq, ExportQuestionEntity aq) => aq.AnswerPoints ?? (object)DBNull.Value),
            ("Q{0}COA", "Competency Area",        typeof(string), null, null,  (ExportData d, Question bq, ExportQuestionEntity aq) => aq.CompetencyAreaValue ?? (object)DBNull.Value),
            ("Q{0}AOM", "Competency",             typeof(string), null, null,  (ExportData d, Question bq, ExportQuestionEntity aq) => aq.CompetencyItemValue ?? (object)DBNull.Value)
        };

        private delegate object GetQuestionColumnValue(ExportData exportData, Question bankQuestion, ExportQuestionEntity attemptQuestion);

        private static object GetResponseText(ExportData d, Question bq, ExportQuestionEntity aq)
        {
            if (aq.AnswerText.IsNotEmpty())
                return aq.AnswerText;

            var options = d.GetQuestionOptions(aq.AttemptIdentifier, aq.QuestionIdentifier).Where(x => x.AnswerIsSelected == true).ToArray();
            if (options.Length > 0)
                return string.Join(" ,", options.OrderBy(x => x.OptionSequence).Select(x => x.OptionText));

            return DBNull.Value;
        }

        private static object GetAnswerOptionSequence(ExportData d, Question bq, ExportQuestionEntity aq)
        {
            if (aq.AnswerOptionKey.HasValue)
            {
                var key = aq.AnswerOptionKey.Value;
                var option = bq.Options.FirstOrDefault(x => x.Number == key);

                if (option != null)
                    return option.Sequence;
            }

            return DBNull.Value;
        }

        private static void AddSetInfo(ExcelWorksheet sheet, ReportData data, SetInfo setInfo)
        {
            var dataTable = BuildDataTable(data, setInfo);
            if (dataTable.Rows.Count == 0)
                return;

            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("LearnerFirstName", "First Name");
            helper.Map("LearnerMiddleName", "Middle Name");
            helper.Map("LearnerLastName", "Last Name");
            helper.Map("LearnerUserEmail", "Email", 20);
            helper.Map("LearnerPersonCode", LabelHelper.GetTranslation("Person Code", "en"));
            helper.Map("LearnerBirthdate", "Date of Birth", "yyyy-MM-dd");
            helper.Map("FormIdentifier", "Form Identifier", 40);
            helper.Map("FormName", "Form Name", 40);
            helper.Map("FormCode", "Form Code");
            helper.Map("SetId", "Set Identifier", 40);
            helper.Map("SetName", "Set Name", 40);

            var questions = setInfo.Questions;
            for (var i = 0; i < questions.Length; i++)
            {
                var q = questions[i];
                foreach (var c in QuestionColumns)
                    helper.Map(c.Column.Format(i), $"Set Q {q.Sequence} {c.Title}", c.Format, c.Width, null);
            }

            helper.ApplyColumnFormatting(sheet, dataTable.DefaultView, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, dataTable.DefaultView, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }

        private static DataTable BuildDataTable(ReportData data, SetInfo setInfo)
        {
            var questions = setInfo.Questions;

            var result = new DataTable();
            result.Columns.Add("LearnerFirstName", typeof(string));
            result.Columns.Add("LearnerMiddleName", typeof(string));
            result.Columns.Add("LearnerLastName", typeof(string));
            result.Columns.Add("LearnerUserEmail", typeof(string));
            result.Columns.Add("LearnerPersonCode", typeof(string));
            result.Columns.Add("LearnerBirthdate", typeof(DateTime));
            result.Columns.Add("FormIdentifier", typeof(Guid));
            result.Columns.Add("FormName", typeof(string));
            result.Columns.Add("FormCode", typeof(string));
            result.Columns.Add("SetId", typeof(Guid));
            result.Columns.Add("SetName", typeof(string));

            for (var i = 0; i < questions.Length; i++)
            {
                var q = questions[i];
                foreach (var c in QuestionColumns)
                    result.Columns.Add(c.Column.Format(i), c.Type);
            }

            foreach (ExportAttemptEntity attempt in data.Attempts)
            {
                var hasAnswers = questions
                    .Any(x => data.Answers.ContainsKey(new MultiKey<Guid, Guid>(attempt.AttemptIdentifier, x.Identifier)));

                if (!hasAnswers)
                    continue;

                var row = result.NewRow();
                row["LearnerFirstName"] = attempt.LearnerFirstName ?? (object)DBNull.Value;
                row["LearnerMiddleName"] = attempt.LearnerMiddleName ?? (object)DBNull.Value;
                row["LearnerLastName"] = attempt.LearnerLastName ?? (object)DBNull.Value;
                row["LearnerUserEmail"] = attempt.LearnerUserEmail ?? (object)DBNull.Value;
                row["LearnerPersonCode"] = attempt.LearnerPersonCode ?? (object)DBNull.Value;
                row["LearnerBirthdate"] = attempt.LearnerBirthdate ?? (object)DBNull.Value;
                row["FormIdentifier"] = attempt.FormIdentifier;
                row["FormName"] = attempt.FormName ?? (object)DBNull.Value;
                row["FormCode"] = attempt.FormCode ?? (object)DBNull.Value;
                row["SetId"] = setInfo.SetId;
                row["SetName"] = setInfo.SetName ?? (object)DBNull.Value;

                for (var i = 0; i < questions.Length; i++)
                {
                    var question = questions[i];
                    var key = new MultiKey<Guid, Guid>(attempt.AttemptIdentifier, question.Identifier);
                    if (!data.Answers.TryGetValue(key, out var aq))
                        continue;

                    foreach (var c in QuestionColumns)
                        row[c.Column.Format(i)] = c.GetValue(data.ExportData, question, aq);
                }

                result.Rows.Add(row);
            }

            return result;
        }

        private static ReportData GetReportData(QAttemptFilter filter)
        {
            var settings = new AttemptAnalysis.Settings(ServiceLocator.AttemptSearch, ServiceLocator.BankSearch);
            {
                var downloadFilter = filter.Clone();
                downloadFilter.CandidateOrganizationIdentifiers = Identity.Organizations.Select(x => x.OrganizationIdentifier).ToArray();
                downloadFilter.OrderBy = null;

                if (downloadFilter is AttemptReportFilter reportFilter && reportFilter.IncludePendingAttempts)
                {
                    downloadFilter.IsSubmitted = true;
                    downloadFilter.IsCompleted = null;
                }
                else
                {
                    downloadFilter.IsCompleted = true;
                }

                settings.Filter = downloadFilter;
                settings.AttemptEntityBinder = ExportAttemptEntity.Binder;
                settings.QuestionEntityBinder = ExportQuestionEntity.Binder;
                settings.OptionEntityBinder = AttemptReportHelper.ExportOptionEntity.Binder;
            }

            var analysis = AttemptAnalysis.Create(settings);
            var data = new AttemptReportHelper.ExportData(analysis);

            return new ReportData
            {
                ExportData = data,

                Attempts = data.Attempts
                    .OrderBy(x => x.AttemptStarted)
                    .ThenBy(x => x.AttemptGraded)
                    .ToArray(),

                Sets = analysis.Questions.Values
                    .GroupBy(x => x.Set.Identifier)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new SetInfo
                        {
                            SetId = g.Key,
                            SetName = first.Set.Name,
                            SetSequence = first.Set.Sequence,
                            Questions = g.OrderBy(q => q.Sequence).ToArray()
                        };
                    })
                    .OrderBy(x => x.SetSequence)
                    .ToArray(),

                Answers = data.Attempts
                    .SelectMany(attempt => data
                        .GetAttemptQuestions(attempt.AttemptIdentifier)
                        .Cast<ExportQuestionEntity>()
                        .Select(q =>
                        (
                            Key: new MultiKey<Guid, Guid>(attempt.AttemptIdentifier, q.QuestionIdentifier),
                            Question: q
                        ))
                    )
                    .ToDictionary(x => x.Key, x => x.Question)
            };
        }
    }
}