using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using InSite.Application.Attempts.Read;
using InSite.Application.Banks.Read;
using InSite.Application.Cases.Write;
using InSite.Application.Files.Read;
using InSite.Persistence;
using InSite.Web.Helpers;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox.Reporting.PerformanceReport;
using Shift.Toolbox.Reporting.PerformanceReport.Models;

namespace InSite.UI.Admin.Assessments.Attempts.Utilities
{
    static class PerformanceReportHelper
    {
        #region Classes

        public class ScoreDetail
        {
            public VPerformanceReport Item { get; private set; }
            public UserScore Score { get; private set; }

            public ScoreDetail(VPerformanceReport item, UserScore score)
            {
                Item = item;
                Score = score;
            }
        }

        #endregion

        #region Save to Case

        public static void SaveReportToCase(ReportConfig reportConfig, VPerformanceReportFilter filter, bool useAlternateFramework, Guid issueId)
        {
            var organizationId = CurrentSessionState.Identity.Organization.Identifier;
            var userId = CurrentSessionState.Identity.User.Identifier;

            var pdf = GetReportPdf(reportConfig, filter, useAlternateFramework);

            var props = CreatePRFileProps(reportConfig, filter, useAlternateFramework, issueId);
            var claims = CreatePRFileClaims();

            FileStorageModel model;

            using (var file = new MemoryStream(pdf))
            {
                model = ServiceLocator.StorageService.Create(
                    file,
                    props.DocumentName,
                    organizationId,
                    userId,
                    issueId,
                    FileObjectType.Issue,
                    props,
                    claims
                );
            }

            ServiceLocator.StorageService.ChangeProperties(model.FileIdentifier, userId, model.Properties, true);

            var command = new AddAttachment(
                issueId,
                model.FileName,
                Path.GetExtension(model.FileName),
                model.FileIdentifier,
                DateTimeOffset.UtcNow,
                userId
            );

            ServiceLocator.SendCommand(command);
        }

        private static FileProperties CreatePRFileProps(ReportConfig reportConfig, VPerformanceReportFilter filter, bool useAlternateFramework, Guid issueId)
        {
            if (filter.AttemptIds == null || filter.AttemptIds.Length == 0)
                throw new ArgumentNullException("AttemptIds is empty");

            var organizationId = CurrentSessionState.Identity.Organization.Identifier;
            var person = ServiceLocator.PersonSearch.GetPerson(filter.LearnerUserIdentifier, organizationId, x => x.User);
            var date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CurrentSessionState.Identity.User.TimeZone);
            var documentName = $"{date:yyyyMMdd} {reportConfig.FileSuffix} PR for {person.PersonCode} {person.User.FullName}";

            documentName = useAlternateFramework ? documentName + " (Alternate).pdf" : documentName + ".pdf";

            var description = new StringBuilder();
            description.Append("Report was generated from these attempts: ");

            for (int i = 0; i < filter.AttemptIds.Length; i++)
            {
                if (i > 0)
                    description.Append(", ");

                description.Append(filter.AttemptIds[i]);
            }

            description.Append(" with the following weights ");

            for (int i = 0; i < reportConfig.AssessmentTypeWeights.Length; i++)
            {
                var item = reportConfig.AssessmentTypeWeights[i];

                if (i > 0)
                    description.Append(" and ");

                description.Append($"{item.Weight:p0} {item.Name}");
            }

            return new FileProperties
            {
                DocumentName = documentName,
                Description = description.ToString(),
                Category = "Performance Report",
                Status = "System Generated",
                AllowLearnerToView = CaseAttachmentHelper.AllowLearnerToViewByIssue(issueId)
            };
        }

        private static IEnumerable<FileClaim> CreatePRFileClaims()
        {
            var claimGroups = CurrentSessionState.Identity.Organization.Toolkits.Issues?.PortalUploadClaimGroups;
            if (claimGroups == null || claimGroups.Length == 0)
                return null;

            var claims = new List<FileClaim>();

            foreach (var group in claimGroups)
            {
                claims.Add(new FileClaim
                {
                    ObjectIdentifier = group,
                    ObjectType = FileClaimObjectType.Group
                });
            }

            return claims;
        }

        #endregion

        #region Create PDF and CSV

        public static byte[] GetReportPdf(ReportConfig reportConfig, VPerformanceReportFilter filter, bool useAlternateFramework)
        {
            var scoresAndSequence = GetScoreDetails(reportConfig, filter, useAlternateFramework);
            var scores = scoresAndSequence.Select(x => x.Score).ToList();

            var reportCreator = new ReportCreator(reportConfig);
            var areaScores = reportCreator.CreateAreaScores(scores);
            var areaIds = areaScores.AreaScores.Select(x => x.AreaId).ToList();
            var areas = GetAreas(reportConfig.Language, areaIds);

            var organizationId = CurrentSessionState.Identity.Organization.Identifier;
            var userId = filter.LearnerUserIdentifier;
            var person = ServiceLocator.PersonSearch.GetPerson(userId, organizationId, x => x.User)
                ?? throw new ArgumentException($"User {userId} is not found in the organization {organizationId}");

            var reportSettings = CurrentSessionState.Identity.Organization.Toolkits.Assessments.PerformanceReport;

            var userReport = new UserReport
            {
                ReportType = GetReportType(reportConfig.RequiredRole, reportConfig.Language),
                FullName = person.User.FullName,
                PersonCode = person.PersonCode,
                ReportIssued = DateTime.Now,
                Areas = areas,
                AreaScores = areaScores,
                NursingRoleText = reportConfig.NursingRoleText,
                Description = reportConfig.Description
            };

            return reportCreator.CreatePdf(userReport);
        }

        private static UserReportType GetReportType(string requiredRole, string language)
        {
            switch (requiredRole.ToString())
            {
                case "HCA":
                    return language == "en" ? UserReportType.Report1 : UserReportType.Report4;
                case "LPN":
                    return language == "en" ? UserReportType.Report2 : UserReportType.Report5;
                case "RN":
                    return language == "en" ? UserReportType.Report3 : UserReportType.Report6;
                default:
                    throw new ArgumentException($"requiredRole: {requiredRole}");
            }
        }

        public static string GetScoresCsv(ReportConfig reportConfig, VPerformanceReportFilter filter, bool useAlternateFramework)
        {
            var scoreDetails = GetScoreDetails(reportConfig, filter, useAlternateFramework);
            var scores = scoreDetails.Select(x => x.Score).ToList();
            var areaIds = scores.Select(x => x.AreaId).Distinct().ToList();
            var areaMap = GetAreas(reportConfig.Language, areaIds).ToDictionary(x => x.Id, x => x.Name);

            scoreDetails = scoreDetails
                .OrderBy(x => x.Score.AreaId)
                .ThenBy(x => x.Score.AssessmentType)
                .ThenByDescending(x => x.Score.Roles.Length)
                .ThenBy(x => x.Item.QuestionSequence)
                .ToList();

            var reportCreator = new ReportCreator(reportConfig);
            var report = reportCreator.CreateAreaScores(scores);
            var areaScores = report.AreaScores;

            var csv = new StringBuilder();
            csv.AppendLine("Area,Assessment Type,Roles,Sequence,Question Code,Question Type,SLA Section Header,Indicator Text,Option Text,Percent,Raw Score,Score,Max Score,Area Score-unweighted,Area Score-weighted,Form Name, Attempt Started");

            AddCsvLines(areaMap, areaScores, reportCreator, csv, scoreDetails);

            return csv.ToString();
        }

        public static List<ScoreDetail> GetScoreDetails(ReportConfig reportConfig, VPerformanceReportFilter filter, bool useAlternateFramework)
        {
            var data = ServiceLocator.PerformanceReportSearch.GetReport(filter);
            var scores = new List<ScoreDetail>();

            foreach (var item in data)
            {
                var assessmentTypeWeight = reportConfig.AssessmentTypeWeights
                    .FirstOrDefault(w => string.Equals(w.Name, item.FormClassificationInstrument, StringComparison.OrdinalIgnoreCase));

                if (assessmentTypeWeight == null)
                    continue;

                var roles = GetRoles(item.QuestionTags);
                if (roles == null || roles.Length == 0)
                    continue;

                var score = new UserScore
                {
                    AreaId = useAlternateFramework && item.Alt_CompetencyAreaIdentifier.HasValue ? item.Alt_CompetencyAreaIdentifier.Value : item.CompetencyAreaIdentifier,
                    AssessmentType = item.FormClassificationInstrument,
                    Roles = roles,
                    MaxScore = item.MaxPoints,
                    Score = item.Points,
                    Graded = TimeZoneInfo.ConvertTime(item.AttemptStarted ?? item.AttemptGraded, CurrentSessionState.Identity.User.TimeZone).DateTime
                };

                scores.Add(new ScoreDetail(item, score));
            }

            return scores;
        }

        private static void AddCsvLines(
            Dictionary<Guid, string> areaMap,
            AreaScore[] areaScores,
            ReportCreator reportCreator,
            StringBuilder csv,
            List<ScoreDetail> scoreDetails
            )
        {
            var prevAreaId = Guid.Empty;
            var (prevWeighted, prevUnweighted) = (0m, 0m);

            foreach (var scoreDetail in scoreDetails)
            {
                var score = scoreDetail.Score;
                var percent = reportCreator.GetScoreWeight(score);
                if (percent == 0)
                    continue;

                var type = GetQuestionType(scoreDetail.Item.QuestionType);
                var isLikert = type == QuestionItemType.Likert;
                var area = areaMap.TryGetValue(score.AreaId, out var rawArea) ? EscapeCsvText(rawArea) : "Unknown";
                var roles = EscapeCsvText(string.Join(",", score.Roles));
                var (weighted, unweighted) = prevAreaId != score.AreaId ? areaScores.FirstOrDefault(x => x.AreaId == score.AreaId).GetScores() : (prevWeighted, prevUnweighted);
                var code = EscapeCsvText(scoreDetail.Item.QuestionCode);
                var typeName = EscapeCsvText(type.GetDescription());
                var parentQuestionText = isLikert ? StripMarkdownHtml(scoreDetail.Item.ParentQuestionText) : null;
                var questionText = isLikert ? StripMarkdownHtml(scoreDetail.Item.QuestionText) : null;
                var optionText = isLikert ? EscapeCsvText(scoreDetail.Item.AnswerOptionText) : null;
                var formName = EscapeCsvText(scoreDetail.Item.FormName);
                var scoreScaled = score.Score * percent;
                var maxScoreScaled = score.MaxScore * percent;

                var attemptStarted = scoreDetail.Item.AttemptStarted.HasValue
                    ? EscapeCsvText(TimeZones.FormatDateOnly(scoreDetail.Item.AttemptStarted.Value, CurrentSessionState.Identity.User.TimeZone))
                    : null;

                csv.Append($"{area},{score.AssessmentType},{roles}");
                csv.Append($",{scoreDetail.Item.QuestionSequence},{code},{typeName},{parentQuestionText},{questionText},{optionText}");
                csv.Append($",{percent:p2},{score.Score:n2},{scoreScaled:n2},{maxScoreScaled:n2},{unweighted:n3},{weighted:n3},{formName},{attemptStarted}");
                csv.AppendLine();

                prevAreaId = score.AreaId;
                (prevWeighted, prevUnweighted) = (weighted, unweighted);
            }
        }

        private static QuestionItemType GetQuestionType(string type)
        {
            return Enum.TryParse<QuestionItemType>(type, out var result)
                ? result
                : throw new ArgumentException($"Unknown question type: {type}");
        }

        private static string StripMarkdownHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = StringHelper.StripMarkdown(text);
            text = StringHelper.StripHtml(text);

            return EscapeCsvText(text);
        }

        private static string EscapeCsvText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var escapedText = text
                .Replace("\"", "\"\"")
                .Replace("\r", "")
                .Replace("\n", " ");

            return text.IndexOf(',') >= 0 || !string.Equals(text, escapedText)
                ? $"\"{escapedText}\""
                : text;
        }

        private static Area[] GetAreas(string language, List<Guid> areaIds)
        {
            return StandardSearch
                .Bind(
                    x => new Area
                    {
                        Id = x.StandardIdentifier,
                        Name = CoreFunctions.GetContentText(x.StandardIdentifier, ContentLabel.Title, language)
                            ?? CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title)
                            ?? x.ContentTitle,
                        Description = CoreFunctions.GetContentText(x.StandardIdentifier, ContentLabel.Description, language)
                            ?? CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Description)
                            ?? x.ContentDescription
                    },
                    x => areaIds.Contains(x.StandardIdentifier),
                    null,
                    nameof(Standard.Sequence)
                ).ToArray();
        }

        private static string[] GetRoles(string questionTags)
        {
            var tags = QBankQuestion.GetQuestionTags(questionTags);
            var frameworkTags = tags.Find(x => string.Equals(x.Item1, "Reporting Tags", StringComparison.OrdinalIgnoreCase));
            return frameworkTags?.Item2?.ToArray();
        }

        #endregion
    }
}