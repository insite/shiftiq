using System;
using System.Linq;
using System.Linq.Expressions;

using InSite.Application.Attempts.Read;
using InSite.Domain.Organizations;

using OfficeOpenXml;
using OfficeOpenXml.Style;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Assessments.Attempts.Utilities
{
    public static class AttemptReportDefaultExport
    {
        #region Classes

        private class ExportAttemptEntity : AttemptAnalysis.AttemptEntity
        {
            public string AttemptGrade { get; private set; }
            public int? AttemptDuration { get; private set; }
            public string AttemptStatus { get; set; }

            public string FormName { get; set; }
            public decimal? FormPoints { get; set; }

            public Guid? AssessorUserIdentifier { get; private set; }
            public Guid? GradingAssessorUserIdentifier { get; private set; }
            public string LearnerFirstName { get; private set; }
            public string LearnerLastName { get; private set; }
            public string LearnerUserEmail { get; private set; }
            public string LearnerPersonCode { get; private set; }

            public string BrowserUserAgent { get; private set; }

            public static new readonly Expression<Func<QAttempt, AttemptAnalysis.AttemptEntity>> Binder = LinqExtensions1.Expr<QAttempt, AttemptAnalysis.AttemptEntity>(x => new ExportAttemptEntity
            {
                AttemptIdentifier = x.AttemptIdentifier,
                FormIdentifier = x.FormIdentifier,
                AssessorUserIdentifier = x.AssessorUserIdentifier,
                GradingAssessorUserIdentifier = x.GradingAssessorUserIdentifier,
                AttemptStarted = x.AttemptStarted,
                AttemptSubmitted = x.AttemptSubmitted,
                AttemptGraded = x.AttemptGraded,
                AttemptNumber = x.AttemptNumber,
                AttemptScore = x.AttemptScore,
                AttemptPoints = x.AttemptPoints,
                AttemptIsPassing = x.AttemptIsPassing,

                AttemptGrade = x.AttemptGrade,
                AttemptDuration = (int?)x.AttemptDuration,
                AttemptStatus = x.AttemptStatus,

                FormName = x.Form.FormName,
                FormPoints = x.FormPoints,

                LearnerUserIdentifier = x.LearnerUserIdentifier,
                LearnerFirstName = x.LearnerUser.UserFirstName,
                LearnerLastName = x.LearnerUser.UserLastName,
                LearnerUserEmail = x.LearnerUser.UserEmail,
                LearnerPersonCode = x.LearnerPerson.PersonCode,

                BrowserUserAgent = x.UserAgent,
            });
        }

        private class ExportMatchEntity
        {
            public string AnswerText { get; private set; }
            public Guid AttemptIdentifier { get; private set; }
            public string MatchLeftText { get; private set; }
            public decimal MatchPoints { get; private set; }
            public string MatchRightText { get; private set; }
            public int MatchSequence { get; private set; }
            public Guid QuestionIdentifier { get; private set; }
            public int QuestionSequence { get; private set; }

            public static readonly Expression<Func<QAttemptMatch, ExportMatchEntity>> Binder = LinqExtensions1.Expr<QAttemptMatch, ExportMatchEntity>(x => new ExportMatchEntity
            {
                AnswerText = x.AnswerText,
                AttemptIdentifier = x.AttemptIdentifier,
                MatchLeftText = x.MatchLeftText,
                MatchPoints = x.MatchPoints,
                MatchRightText = x.MatchRightText,
                MatchSequence = x.MatchSequence,
                QuestionIdentifier = x.QuestionIdentifier,
                QuestionSequence = x.QuestionSequence
            });
        }

        #endregion

        #region Properties

        private static Domain.Foundations.ISecurityFramework Identity => CurrentSessionState.Identity;
        private static OrganizationState Organization => Identity.Organization;

        #endregion

        public static byte[] GetXlsx(QAttemptFilter filter, bool includeAdditionalSheets)
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
                settings.QuestionEntityBinder = AttemptReportHelper.ExportQuestionEntity.Binder;
                settings.OptionEntityBinder = AttemptReportHelper.ExportOptionEntity.Binder;
            }

            var analysis = AttemptAnalysis.Create(settings);
            var data = new AttemptReportHelper.ExportData(analysis);

            using (var excel = new ExcelPackage())
            {
                var defaultStyle = excel.Workbook.Styles.CellStyleXfs[0];
                defaultStyle.Font.Name = "Calibri";
                defaultStyle.Font.Size = 11;
                defaultStyle.VerticalAlignment = ExcelVerticalAlignment.Top;

                var headerStyle = excel.Workbook.Styles.CreateNamedStyle("Header");
                headerStyle.Style.Font.Bold = true;

                var questionStyle = excel.Workbook.Styles.CreateNamedStyle("Question");
                questionStyle.Style.Font.Italic = true;

                AddSheetAttempts(excel, data);

                if (includeAdditionalSheets)
                {
                    AddSheetAnswers(excel, data);
                    AddSheetSelections(excel, data);
                    AddSheetMatches(excel, data, filter);
                    AddSheetPins(excel, data, filter);
                }

                excel.Workbook.Properties.Title = "Exam Attempt Report";
                excel.Workbook.Properties.Company = CurrentSessionState.Identity.Organization.CompanyName;
                excel.Workbook.Properties.Author = CurrentSessionState.Identity.User.FullName;
                excel.Workbook.Properties.Created = DateTimeOffset.Now.DateTime;

                return excel.GetAsByteArray();
            }
        }

        private static void AddSheetAttempts(ExcelPackage excel, AttemptReportHelper.ExportData data)
        {
            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("AssessorUserIdentifier", "Assessor User Identifier");
            helper.Map("GradingAssessorUserIdentifier", "Grading Assessor User Identifier");
            helper.Map("AttemptDuration", "Attempt Duration");
            helper.Map("AttemptGrade", "Attempt Grade");
            helper.Map("AttemptGraded", "Attempt Graded", "yyyy-MM-dd HH:mm");
            helper.Map("AttemptIdentifier", "Attempt Identifier");
            helper.Map("AttemptIsPassing", "Attempt Is Passing");
            helper.Map("AttemptNumber", "Attempt Number");
            helper.Map("AttemptPoints", "Attempt Points");
            helper.Map("AttemptScore", "Attempt Score", "0.00%");
            helper.Map("AttemptStarted", "Attempt Started", "yyyy-MM-dd HH:mm");
            helper.Map("AttemptStatus", "Attempt Status");
            helper.Map("AttemptSubmitted", "Attempt Submitted", "yyyy-MM-dd HH:mm");
            helper.Map("FormName", "Form Name");
            helper.Map("FormPoints", "Form Points");
            helper.Map("LearnerUserIdentifier", "Learner User Identifier");
            helper.Map("LearnerFirstName", "Learner User First Name");
            helper.Map("LearnerLastName", "Learner User Last Name");
            helper.Map("LearnerUserEmail", "Learner User Email");
            helper.Map("LearnerPersonCode", "Learner Person Code");
            helper.Map("BrowserUserAgent", "Browser User Agent");

            var sheet = excel.Workbook.Worksheets.Add("Attempts");
            sheet.Cells.Style.WrapText = true;

            helper.ApplyColumnFormatting(sheet, data.Attempts, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, data.Attempts, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }

        private static void AddSheetAnswers(ExcelPackage excel, AttemptReportHelper.ExportData data)
        {
            var orderingType = QuestionItemType.Ordering.GetName();
            var dataSource = data.EnumerateAllQuestions()
                .Select(x => new
                {
                    AnswerOptionKey = x.AnswerOptionKey,
                    QuestionSetName = x.QuestionSetName,
                    QuestionSetId = x.QuestionSetId,
                    QuestionText = x.QuestionText,
                    AnswerOptionSequence = x.AnswerOptionSequence,
                    QuestionSequence = x.QuestionSequence,
                    AnswerPoints = x.QuestionType == orderingType ? x.AnswerPoints ?? 0 : x.AnswerPoints,
                    AnswerText = x.AnswerText,
                    AnswerFileIdentifier = x.AnswerFileIdentifier,
                    AttemptIdentifier = x.AttemptIdentifier,
                    QuestionCalculationMethod = x.QuestionCalculationMethod,
                    QuestionIdentifier = x.QuestionIdentifier,
                    ParentQuestionIdentifier = x.ParentQuestionIdentifier,
                    QuestionPoints = x.QuestionPoints,
                    QuestionType = x.QuestionType
                })
                .ToArray();
            if (dataSource.Length == 0)
                return;

            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("AnswerOptionKey", "Answer Option Key");
            helper.Map("QuestionSetName", "Set Name");
            helper.Map("QuestionSetId", "Set Identifier");
            helper.Map("QuestionText", "Question Text");
            helper.Map("AnswerOptionSequence", "Answer Option Sequence");
            helper.Map("QuestionSequence", "Question Sequence");
            helper.Map("AnswerPoints", "Answer Points");
            helper.Map("AnswerText", "Answer Text");
            helper.Map("AnswerFileIdentifier", "Answer File Identifier");
            helper.Map("AttemptIdentifier", "Attempt Identifier");
            helper.Map("QuestionCalculationMethod", "Question Calculation Method");
            helper.Map("QuestionIdentifier", "Question Identifier");
            helper.Map("ParentQuestionIdentifier", "Parent Question Identifier");
            helper.Map("QuestionPoints", "Question Points");
            helper.Map("QuestionType", "Question Type");

            var sheet = excel.Workbook.Worksheets.Add("Answers");
            sheet.Cells.Style.WrapText = true;

            helper.ApplyColumnFormatting(sheet, dataSource, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, dataSource, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }

        private static void AddSheetSelections(ExcelPackage excel, AttemptReportHelper.ExportData data)
        {
            var dataSource = data.EnumerateAllQuestions()
                .SelectMany(q => data.GetQuestionOptions(q.AttemptIdentifier, q.QuestionIdentifier)
                    .Select(o => new
                    {
                        q.QuestionSetName,
                        q.QuestionSetId,

                        o.AttemptIdentifier,
                        q.QuestionIdentifier,
                        q.ParentQuestionIdentifier,
                        o.QuestionSequence,
                        o.OptionKey,
                        o.OptionSequence,
                        o.OptionPoints,
                        o.AnswerIsSelected,
                        o.OptionAnswerSequence,

                        o.OptionText,
                        o.OptionIsTrue
                    }))
                .ToArray();

            if (dataSource.Length == 0)
                return;

            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("QuestionSetName", "Set Name");
            helper.Map("QuestionSetId", "Set Identifier");
            helper.Map("QuestionSequence", "Question Sequence");
            helper.Map("AnswerIsSelected", "Answer Is Selected");
            helper.Map("AttemptIdentifier", "Attempt Identifier");
            helper.Map("OptionIsTrue", "Option Is True");
            helper.Map("OptionKey", "Option Key");
            helper.Map("OptionPoints", "Option Points");
            helper.Map("OptionSequence", "Option Sequence");
            helper.Map("OptionText", "Option Text");
            helper.Map("OptionAnswerSequence", "Option Answer Sequence");
            helper.Map("QuestionIdentifier", "Question Identifier");
            helper.Map("ParentQuestionIdentifier", "Parent Question Identifier");

            var sheet = excel.Workbook.Worksheets.Add("Selections");
            sheet.Cells.Style.WrapText = true;

            helper.ApplyColumnFormatting(sheet, dataSource, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, dataSource, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }

        private static void AddSheetMatches(ExcelPackage excel, AttemptReportHelper.ExportData data, QAttemptFilter filter)
        {
            var matchesByAttempt = ServiceLocator.AttemptSearch.BindAttemptMatches(ExportMatchEntity.Binder, filter)
                .OrderBy(x => x.QuestionSequence).ThenBy(x => x.MatchSequence).GroupBy(x => x.AttemptIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => x.GroupBy(y => y.QuestionIdentifier).ToDictionary(y => y.Key, y => y.ToArray()));

            if (matchesByAttempt.Count == 0)
                return;

            var dataSource = data.EnumerateAllQuestions()
                .SelectMany(q =>
                {
                    ExportMatchEntity[] matches;

                    if (!matchesByAttempt.TryGetValue(q.AttemptIdentifier, out var matchesByQuestion))
                        matches = new ExportMatchEntity[0];
                    else if (!matchesByQuestion.TryGetValue(q.QuestionIdentifier, out matches))
                        matches = new ExportMatchEntity[0];

                    return matches.Select(m => new
                    {
                        q.QuestionSetName,
                        q.QuestionSetId,

                        m.AnswerText,
                        m.AttemptIdentifier,
                        m.MatchLeftText,
                        m.MatchPoints,
                        m.MatchRightText,
                        m.MatchSequence,
                        q.QuestionIdentifier,
                        m.QuestionSequence
                    });
                })
                .ToArray();

            if (dataSource.Length == 0)
                return;

            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("QuestionSetName", "Set Name");
            helper.Map("QuestionSetId", "Set Identifier");
            helper.Map("AnswerText", "Answer Text");
            helper.Map("AttemptIdentifier", "Attempt Identifier");
            helper.Map("MatchLeftText", "Match Left Text");
            helper.Map("MatchPoints", "Match Points");
            helper.Map("MatchRightText", "Match Right Text");
            helper.Map("MatchSequence", "Match Sequence");
            helper.Map("QuestionIdentifier", "Question Identifier");

            var sheet = excel.Workbook.Worksheets.Add("Matches");
            sheet.Cells.Style.WrapText = true;

            helper.ApplyColumnFormatting(sheet, dataSource, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, dataSource, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }

        private static void AddSheetPins(ExcelPackage excel, AttemptReportHelper.ExportData data, QAttemptFilter filter)
        {
            var pinsByAttempt = ServiceLocator.AttemptSearch.BindAttemptPins(x => x, filter)
                .OrderBy(x => x.QuestionSequence)
                .ThenBy(x => x.PinSequence)
                .GroupBy(x => x.AttemptIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => x.GroupBy(y => y.QuestionIdentifier).ToDictionary(y => y.Key, y => y.ToArray()));

            if (pinsByAttempt.Count == 0)
                return;

            var dataSource = data.EnumerateAllQuestions()
                .SelectMany(q =>
                {
                    QAttemptPin[] pins;

                    if (!pinsByAttempt.TryGetValue(q.AttemptIdentifier, out var pinsByQuestion))
                        pins = new QAttemptPin[0];
                    else if (!pinsByQuestion.TryGetValue(q.QuestionIdentifier, out pins))
                        pins = new QAttemptPin[0];

                    return pins.Select(p => new
                    {
                        q.QuestionSetName,
                        q.QuestionSetId,

                        p.AttemptIdentifier,
                        q.QuestionIdentifier,
                        p.QuestionSequence,
                        p.OptionKey,
                        p.OptionPoints,
                        p.OptionSequence,
                        p.OptionText,
                        p.PinSequence,
                        p.PinX,
                        p.PinY
                    });
                })
                .ToArray();

            if (dataSource.Length == 0)
                return;

            var helper = new XlsxExportHelper(CurrentSessionState.Identity.User.TimeZone);

            helper.Map("QuestionSetName", "Set Name");
            helper.Map("QuestionSetId", "Set Identifier");
            helper.Map("QuestionSequence", "Question Sequence");
            helper.Map("AttemptIdentifier", "Attempt Identifier");
            helper.Map("OptionKey", "Option Key");
            helper.Map("OptionPoints", "Option Points");
            helper.Map("OptionSequence", "Option Sequence");
            helper.Map("OptionText", "Option Text");
            helper.Map("PinSequence", "Pin Sequence");
            helper.Map("PinX", "Pin X");
            helper.Map("PinY", "Pin Y");
            helper.Map("QuestionIdentifier", "Question Identifier");

            var sheet = excel.Workbook.Worksheets.Add("Hotspot Pins");
            sheet.Cells.Style.WrapText = true;

            helper.ApplyColumnFormatting(sheet, dataSource, 1);
            helper.InsertHeader(sheet, 1, 1, false);
            helper.InsertData(sheet, dataSource, 2, 1, false);
            helper.ApplyColumnWidth(sheet, 1, true);
        }
    }
}