using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Toolbox.Reporting.AssessmentAudit.Models;

using PerformanceAreaScoreResult = Shift.Toolbox.Reporting.PerformanceReport.Models.AreaScoreResult;
using PerformanceReportCreator = Shift.Toolbox.Reporting.PerformanceReport.ReportCreator;
using ReportConfig = Shift.Toolbox.Reporting.PerformanceReport.Models.ReportConfig;
using UserScore = Shift.Toolbox.Reporting.PerformanceReport.Models.UserScore;

namespace Shift.Toolbox.Reporting.AssessmentAudit
{
    public class ReportCreator
    {
        private readonly ReportConfig[] _reportConfigs;

        private IEnumerable<UserScore> _scores;

        private List<PerformanceAreaScoreResult> _performanceReports;
        private Dictionary<string, List<AreaScore>> _areaScoresPerAssessmentType;

        public ReportCreator(ReportConfig[] reportConfigs)
        {
            _reportConfigs = reportConfigs;
        }

        public List<(string AssessmentType, List<AreaScore> AreaScores)> CreateReport(IEnumerable<UserScore> scores)
        {
            _scores = scores;
            _areaScoresPerAssessmentType = new Dictionary<string, List<AreaScore>>();

            CreatePerformanceReports();
            AddAreaScores();

            var result = new List<(string AssessmentType, List<AreaScore> AreaScores)>();
            foreach (var assessmentType in _areaScoresPerAssessmentType.Keys)
                result.Add((assessmentType, _areaScoresPerAssessmentType[assessmentType]));

            return result;
        }

        private void AddAreaScores()
        {
            var areaIds = _performanceReports.SelectMany(x => x.AreaScores.Select(y => y.AreaId)).Distinct();
            foreach (var areaId in areaIds)
            {
                for (int i = 0; i < _reportConfigs.Length; i++)
                {
                    var requiredRole = _reportConfigs[i].RequiredRole;
                    var areaRoleScores = GetAreaRoleScores(areaId, requiredRole);

                    foreach (var assessmentType in areaRoleScores.Keys)
                    {
                        if (!_areaScoresPerAssessmentType.TryGetValue(assessmentType, out var areaScores))
                            _areaScoresPerAssessmentType.Add(assessmentType, areaScores = new List<AreaScore>());

                        areaScores.Add(new AreaScore
                        {
                            AreaId = areaId,
                            RequiredRole = requiredRole,
                            RoleScores = areaRoleScores[assessmentType].ToArray()
                        });
                    }
                }
            }
        }

        private Dictionary<string, List<RoleScore>> GetAreaRoleScores(Guid areaId, string requiredRole)
        {
            var result = new Dictionary<string, List<RoleScore>>();

            for (int i = 0; i < _reportConfigs.Length; i++)
            {
                var role = _reportConfigs[i].RequiredRole;
                var areaScore = _performanceReports[i].AreaScores.FirstOrDefault(x => x.AreaId == areaId);

                if (areaScore == null)
                    continue;

                foreach (var assessmentTypeScore in areaScore.AssessmentTypeScores)
                {
                    var roleScore = assessmentTypeScore.RoleScores.FirstOrDefault(x => string.Equals(x.Name, requiredRole, StringComparison.OrdinalIgnoreCase));
                    if (roleScore == null)
                        continue;

                    if (!result.TryGetValue(assessmentTypeScore.Name, out var score))
                        result.Add(assessmentTypeScore.Name, score = new List<RoleScore>());

                    score.Add(new RoleScore
                    {
                        Role = role,
                        Score = roleScore.WeightedScore,
                        MaxScore = roleScore.WeightedMaxScore
                    });
                }
            }

            return result;
        }

        private void CreatePerformanceReports()
        {
            _performanceReports = new List<PerformanceAreaScoreResult>();

            foreach (var reportConfig in _reportConfigs)
            {
                var reportResult = new PerformanceReportCreator(reportConfig).CreateAreaScores(_scores);
                _performanceReports.Add(reportResult);
            }
        }
    }
}
