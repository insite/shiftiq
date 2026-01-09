using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Domain.Organizations.PerformanceReport;
using InSite.Persistence;

using Shift.Constant;
using Shift.Toolbox;
using Shift.Toolbox.Reporting.AssessmentAudit.Models;

using ColorTranslator = System.Drawing.ColorTranslator;
using DomainItemWeight = InSite.Domain.Organizations.PerformanceReport.ItemWeight;
using DrawingColor = System.Drawing.Color;
using ItemWeight = Shift.Toolbox.Reporting.PerformanceReport.Models.ItemWeight;
using ReportConfig = Shift.Toolbox.Reporting.PerformanceReport.Models.ReportConfig;
using ReportCreator = Shift.Toolbox.Reporting.AssessmentAudit.ReportCreator;
using UserScore = Shift.Toolbox.Reporting.PerformanceReport.Models.UserScore;

namespace InSite.UI.Admin.Assessments.Attempts.Utilities
{
    internal class AssessmentAuditReport
    {
        class ReportScore
        {
            public object Score { get; set; }
            public object MaxScore { get; set; }
            public object ReportedScore { get; set; }
            public bool HasScore { get; set; }
        }

        class RoleColumn
        {
            public string Name { get; set; }
            public DrawingColor BackgroundColor { get; set; }
        }

        private static readonly DrawingColor[] ColumnHeaderColors = new[]
        {
            ColorTranslator.FromHtml("#C1F0C8"),
            ColorTranslator.FromHtml("#8ED973"),
            ColorTranslator.FromHtml("#3C7D22")
        };

        private static readonly XlsxCellStyle HeaderLeftStyle = new XlsxCellStyle { IsBold = true };
        private static readonly XlsxCellStyle HeaderLeftMarkedStyle = new XlsxCellStyle { IsBold = true, BackgroundColor = ColorTranslator.FromHtml("#FFFF00") };
        private static readonly XlsxCellStyle DataLeftStyle = new XlsxCellStyle();
        private static readonly XlsxCellStyle DataLeftMarkedStyle = new XlsxCellStyle { BackgroundColor = ColorTranslator.FromHtml("#FFFF00") };
        private static readonly XlsxCellStyle DataRightStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right };
        private static readonly XlsxCellStyle DataRightMarkedStyle = new XlsxCellStyle { Align = HorizontalAlignment.Right, BackgroundColor = ColorTranslator.FromHtml("#FFFF00") };

        private static readonly string ScoreFormat = "#,##0.00";

        private readonly Guid _userId;
        private readonly string _personCode;
        private readonly List<UserScore> _scores;
        private readonly bool _isAlternate;
        private readonly List<SearchPerformanceReport.ReportItem> _reports;

        private List<(Guid Id, string Name)> _areas;
        private string _assessmentType;
        private List<AreaScore> _areaScores;

        private int HeaderColumns => _isAlternate ? 5 : 7;

        static AssessmentAuditReport()
        {
            HeaderLeftStyle.Border.BorderAround(XlsxBorderStyle.None);
            HeaderLeftMarkedStyle.Border.BorderAround(XlsxBorderStyle.None);
            DataLeftStyle.Border.BorderAround(XlsxBorderStyle.None);
            DataLeftMarkedStyle.Border.BorderAround(XlsxBorderStyle.None);
            DataRightStyle.Border.BorderAround(XlsxBorderStyle.None);
            DataRightMarkedStyle.Border.BorderAround(XlsxBorderStyle.None);
        }

        private AssessmentAuditReport(Guid userId, string personCode, List<UserScore> scores, bool isAlternate)
        {
            _userId = userId;
            _personCode = personCode;
            _scores = scores;
            _isAlternate = isAlternate;
            _reports = SearchPerformanceReport.GetReports();
        }

        public static byte[] GetXlsx(Guid userId, string personCode, List<UserScore> scores, bool isAlternate)
        {
            return new AssessmentAuditReport(userId, personCode, scores, isAlternate).GetXlsx();
        }

        private byte[] GetXlsx()
        {
            var reportConfigs = CreateReportConfigs();
            var reportScores = new ReportCreator(reportConfigs).CreateReport(_scores);

            reportScores.Sort((a, b) => a.AssessmentType.CompareTo(b.AssessmentType));

            ReadAreas(reportScores);

            var sheets = new List<XlsxWorksheet>();

            foreach (var (assessmentType, areaScores) in reportScores)
            {
                _assessmentType = assessmentType;
                _areaScores = areaScores;

                var sheet = CreateSheet();
                AddDataToSheet(sheet, 1);

                sheets.Add(sheet);
            }

            return XlsxWorksheet.GetBytes(sheets.ToArray());
        }

        private XlsxWorksheet CreateSheet()
        {
            var sheet = new XlsxWorksheet($"{_assessmentType} reported scores");

            sheet.Columns[0].Width = 37; // Learner Identifier
            sheet.Columns[1].Width = 16; // Person Code
            sheet.Columns[2].Width = 16; // Assessment Type
            sheet.Columns[3].Width = 37; // Area
            sheet.Columns[4].Width = 10; // Set Role

            if (!_isAlternate)
            {
                sheet.Columns[5].Width = 25; // Total Max Score for Set Role
                sheet.Columns[6].Width = 22; // Total Scores for Set Role
            }

            var columnCount = HeaderColumns + 3 * _reports.Count;
            for (int i = HeaderColumns; i < columnCount; i++)
                sheet.Columns[i].Width = 28.5;

            sheet.Cells.Add(new XlsxCell(0, 0) { Value = "Learner Identifier (ShiftIQ code)", Style = HeaderLeftMarkedStyle });
            sheet.Cells.Add(new XlsxCell(1, 0) { Value = "Person Code", Style = HeaderLeftMarkedStyle });
            sheet.Cells.Add(new XlsxCell(2, 0) { Value = "Assessment Type", Style = HeaderLeftStyle });
            sheet.Cells.Add(new XlsxCell(3, 0) { Value = _isAlternate ? "Reporting Framework Area Name" : "Area", Style = HeaderLeftMarkedStyle });
            sheet.Cells.Add(new XlsxCell(4, 0) { Value = "Set Role", Style = HeaderLeftStyle });

            if (!_isAlternate)
            {
                sheet.Cells.Add(new XlsxCell(5, 0) { Value = "Total Max Score for Set Role", Style = HeaderLeftStyle });
                sheet.Cells.Add(new XlsxCell(6, 0) { Value = "Total Scores for Set Role", Style = HeaderLeftStyle });
            }

            for (int i = 0; i < _reports.Count; i++)
            {
                var startCol = HeaderColumns + i * 3;

                var requiredRole = _reports[i].RequiredRole;
                var roleColumn = GetRoleColumn(i);
                var columnName = roleColumn?.Name ?? requiredRole;
                var cellStyle = new XlsxCellStyle { IsBold = true, BackgroundColor = roleColumn?.BackgroundColor };

                cellStyle.Border.BorderAround(XlsxBorderStyle.None);

                sheet.Cells.Add(new XlsxCell(startCol, 0) { Value = _isAlternate ? "Max Score" : $"Weighted Max {columnName}", Style = cellStyle });
                sheet.Cells.Add(new XlsxCell(startCol + 1, 0) { Value = _isAlternate ? "Score" : $"Weighted Score {columnName}", Style = cellStyle });
                sheet.Cells.Add(new XlsxCell(startCol + 2, 0) { Value = _isAlternate ? "Report Score" : $"Report Score {columnName}", Style = cellStyle });

                if (_isAlternate)
                    break;
            }

            return sheet;
        }

        private void AddDataToSheet(XlsxWorksheet sheet, int startRow)
        {
            var sortedAreaScores = _areaScores
                .Select(areaScore => new
                {
                    AreaName = _areas.Find(x => x.Id == areaScore.AreaId).Name,
                    AreaScore = areaScore
                })
                .OrderBy(x => x.AreaName)
                .ThenBy(x => x.AreaScore.RequiredRole)
                .ToList();

            var row = startRow;

            foreach (var areaAndScore in sortedAreaScores)
            {
                var areaName = areaAndScore.AreaName;
                var areaScore = areaAndScore.AreaScore;
                var requiredRole = areaScore.RequiredRole;
                var reportScore = GetReportScore(areaScore, requiredRole);

                if (_isAlternate && !string.Equals(requiredRole, "HCA", StringComparison.OrdinalIgnoreCase))
                    continue;

                sheet.Cells.Add(new XlsxCell(0, row) { Value = _userId.ToString().ToLower(), Style = DataLeftMarkedStyle });
                sheet.Cells.Add(new XlsxCell(1, row) { Value = _personCode, Style = DataLeftMarkedStyle });
                sheet.Cells.Add(new XlsxCell(2, row) { Value = _assessmentType, Style = DataLeftStyle });
                sheet.Cells.Add(new XlsxCell(3, row) { Value = areaName, Style = DataLeftMarkedStyle });
                sheet.Cells.Add(new XlsxCell(4, row) { Value = requiredRole, Style = DataLeftStyle });

                if (!_isAlternate)
                {
                    sheet.Cells.Add(new XlsxCell(5, row) { Value = reportScore.MaxScore, Style = DataRightStyle, Format = ScoreFormat });
                    sheet.Cells.Add(new XlsxCell(6, row) { Value = reportScore.Score, Style = DataRightStyle, Format = ScoreFormat });
                }

                AddReportResultsToSheet(sheet, row, areaScore);

                row++;
            }
        }

        private void AddReportResultsToSheet(XlsxWorksheet sheet, int row, AreaScore areaScore)
        {
            var startCol = HeaderColumns;

            foreach (var report in _reports)
            {
                var reportScore = GetReportScore(areaScore, report.RequiredRole);
                var reportedScoreStyle = reportScore.HasScore ? DataRightMarkedStyle : DataRightStyle;

                sheet.Cells.Add(new XlsxCell(startCol, row) { Value = reportScore.MaxScore, Style = DataRightStyle, Format = ScoreFormat });
                sheet.Cells.Add(new XlsxCell(startCol + 1, row) { Value = reportScore.Score, Style = DataRightStyle, Format = ScoreFormat });
                sheet.Cells.Add(new XlsxCell(startCol + 2, row) { Value = reportScore.ReportedScore, Style = reportedScoreStyle, Format = ScoreFormat });

                if (_isAlternate)
                    break;

                startCol += 3;
            }
        }

        private ReportScore GetReportScore(AreaScore areaScore, string role)
        {
            var roleScore = areaScore.RoleScores.FirstOrDefault(x => string.Equals(x.Role, role, StringComparison.OrdinalIgnoreCase));
            var score = roleScore?.Score != null ? (object)roleScore.Score : "n/a";
            var maxScore = roleScore?.MaxScore != null ? (object)roleScore.MaxScore : "n/a";
            var reportedScore = CalcAreaScore(areaScore.AreaId, role);

            return new ReportScore
            {
                Score = score,
                MaxScore = maxScore,
                ReportedScore = reportedScore,
                HasScore = roleScore?.Score != null
            };
        }

        private decimal CalcAreaScore(Guid areaId, string role)
        {
            var scoreSum = 0m;
            var maxScoreSum = 0m;

            foreach (var areaScore in _areaScores)
            {
                if (areaScore.AreaId != areaId)
                    continue;

                var roleScore = areaScore.RoleScores.FirstOrDefault(x => string.Equals(x.Role, role));
                if (roleScore == null)
                    continue;

                scoreSum += roleScore.Score ?? 0;
                maxScoreSum += roleScore.MaxScore ?? 0;
            }

            return maxScoreSum != 0 ? scoreSum / maxScoreSum : 0;
        }

        private void ReadAreas(List<(string AssessmentType, List<AreaScore> AreaScores)> reportScores)
        {
            var areaIds = reportScores.SelectMany(x => x.AreaScores.Select(y => y.AreaId)).Distinct();

            _areas = StandardSearch
                .Bind(
                    x => new
                    {
                        x.StandardIdentifier,
                        x.ContentTitle
                    },
                    x => areaIds.Contains(x.StandardIdentifier)
                )
                .OrderBy(x => x.ContentTitle)
                .Select(x => (x.StandardIdentifier, x.ContentTitle))
                .ToList();
        }

        private ReportConfig[] CreateReportConfigs()
        {
            var reportConfigs = new List<ReportConfig>();

            foreach (var report in _reports)
            {
                var reportConfig = CreateReportConfig(report);
                if (string.Equals(reportConfig.Language, "en", StringComparison.OrdinalIgnoreCase))
                    reportConfigs.Add(reportConfig);
            }

            return reportConfigs.ToArray();
        }

        private ReportConfig CreateReportConfig(SearchPerformanceReport.ReportItem report)
        {
            var roleWeights = report.Roles
                .Select(role => new ItemWeight
                {
                    Name = role.Name,
                    Weight = role.Weight
                })
                .ToArray();

            var assessmentTypeWeights = report.AssessmentTypes
                .Select(x => new ItemWeight
                {
                    Name = x.Name,
                    Weight = x.Weight
                })
                .ToArray();

            return new ReportConfig
            {
                Language = GetLanguageCode(report.Language),
                FileSuffix = report.FileSuffix,
                EmergentScore = report.EmergentScore,
                ConsistentScore = report.ConsistentScore,
                RequiredRole = report.RequiredRole,
                RoleWeights = roleWeights,
                AssessmentTypeWeights = assessmentTypeWeights,
                NursingRoleText = report.NursingRoleText,
                Description = report.Description
            };
        }

        private static string GetLanguageCode(string languageName)
        {
            return string.Equals(languageName, "English", StringComparison.OrdinalIgnoreCase)
                ? "en"
                : string.Equals(languageName, "French", StringComparison.OrdinalIgnoreCase)
                    ? "fr"
                    : throw new ArgumentException($"languageName: {languageName}");
        }

        private RoleColumn GetRoleColumn(int reportIndex)
        {
            var report = _reports[reportIndex];
            var name = string.Join("+", report.Roles.Select(x => x.Name));

            if (report.Roles.Length == 1)
                name += " only";

            var backgroundColorIndex = reportIndex % ColumnHeaderColors.Length;

            return new RoleColumn
            {
                Name = name,
                BackgroundColor = ColumnHeaderColors[backgroundColorIndex]
            };
        }
    }
}