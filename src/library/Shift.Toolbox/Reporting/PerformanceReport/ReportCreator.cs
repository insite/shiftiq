using System;
using System.Collections.Generic;
using System.Linq;

using QuestPDF.Fluent;

using Shift.Toolbox.Reporting.PerformanceReport.Models;
using Shift.Toolbox.Reporting.PerformanceReport.NCAS;

namespace Shift.Toolbox.Reporting.PerformanceReport
{
    public class ReportCreator
    {
        private class AssessmentTypeScores
        {
            public string Name { get; set; }
            public List<UserScore> Scores { get; set; }
        }

        private class AreaAssessmentTypes
        {
            public Guid AreaId { get; set; }
            public List<AssessmentTypeScores> AssessmentTypeScores { get; set; }
        }

        private readonly ReportConfig _config;

        public ReportCreator(ReportConfig config)
        {
            if (string.IsNullOrEmpty(config?.RequiredRole))
                throw new ArgumentNullException("config.RequiredRole");

            if (config?.RoleWeights == null)
                throw new ArgumentNullException("config.RoleWeights");

            if (config?.AssessmentTypeWeights == null)
                throw new ArgumentNullException("config.AssessmentTypeWeights");

            _config = config;
        }

        public byte[] CreatePdf(UserReport userReport)
        {
            var maxScore = _config.AssessmentTypeWeights.Sum(x => x.Weight);

            return new ReportDocument(_config.Language, _config.EmergentScore, _config.ConsistentScore, maxScore, userReport).GeneratePdf();
        }

        public AreaScoreResult CreateAreaScores(IEnumerable<UserScore> userScores)
        {
            if (userScores == null)
                throw new ArgumentNullException("userScores");

            var areaScores = new List<AreaScore>();
            var areas = GetAreas(userScores);

            foreach (var area in areas)
            {
                var assessmentTypeScores = CalcAssessmentTypeScores(area);

                areaScores.Add(new AreaScore
                {
                    AreaId = area.AreaId,
                    AssessmentTypeScores = assessmentTypeScores
                });
            }

            var assessmentTypeDates = userScores
                .GroupBy(x => x.AssessmentType.ToUpper())
                .Select(g => new AssessmentTypeDate
                {
                    AssessmentType = g.Key,
                    Date = g.Min(score => score.Graded)
                })
                .ToArray();

            return new AreaScoreResult
            {
                AreaScores = areaScores.ToArray(),
                AssessmentTypeDates = assessmentTypeDates,
            };
        }

        public decimal GetScoreWeight(UserScore userScore)
        {
            if (!IsScoreAccepted(userScore))
                return 0;

            return GetScoreWeightInternal(userScore)?.Weight ?? 1m;
        }

        private ItemWeight GetScoreWeightInternal(UserScore userScore)
        {
            if (userScore.Roles == null || userScore.Roles.Length == 0)
                throw new ArgumentException("userScore.Roles is empty");

            ItemWeight maxWeight = null;

            foreach (var role in userScore.Roles)
            {
                var roleWeight = _config.RoleWeights.FirstOrDefault(w => string.Equals(w.Name, role, StringComparison.OrdinalIgnoreCase));

                if (roleWeight != null && (maxWeight == null || roleWeight.Weight > maxWeight.Weight))
                    maxWeight = roleWeight;
            }

            return maxWeight;
        }

        private AssessmentTypeScore[] CalcAssessmentTypeScores(AreaAssessmentTypes area)
        {
            var assessmentTypes = new List<AssessmentTypeScore>();

            foreach (var at in area.AssessmentTypeScores)
            {
                var assessmentTypeWeight = _config.AssessmentTypeWeights.FirstOrDefault(w => string.Equals(w.Name, at.Name, StringComparison.OrdinalIgnoreCase))
                    ?? throw new ArgumentException($"AssessmentType '{at.Name}' is not found");

                var roleScores = CalcRoleScores(at.Scores);

                assessmentTypes.Add(new AssessmentTypeScore
                {
                    Name = at.Name,
                    Weight = assessmentTypeWeight.Weight,
                    RoleScores = roleScores
                });
            }

            return assessmentTypes.ToArray();
        }

        private RoleScore[] CalcRoleScores(List<UserScore> userScores)
        {
            var roles = new List<RoleScore>();

            foreach (var userScore in userScores)
            {
                var roleWeight = GetScoreWeightInternal(userScore);

                var role = roles.Find(x => string.Equals(x.Name, roleWeight.Name));
                if (role == null)
                {
                    roles.Add(role = new RoleScore
                    {
                        Name = roleWeight.Name,
                        Weight = roleWeight.Weight
                    });
                }

                role.WeightedScore += roleWeight.Weight * userScore.Score;
                role.WeightedMaxScore += roleWeight.Weight * userScore.MaxScore;
            }

            return roles.ToArray();
        }

        private List<AreaAssessmentTypes> GetAreas(IEnumerable<UserScore> userScores)
        {
            var filteredScores = userScores
                .Where(userScore => IsScoreAccepted(userScore))
                .ToList();

            var areas = filteredScores
                .GroupBy(userScore => userScore.AreaId)
                .Select(areaGroup => new AreaAssessmentTypes
                {
                    AreaId = areaGroup.Key,
                    AssessmentTypeScores = areaGroup
                        .GroupBy(userScore => userScore.AssessmentType.ToUpper())
                        .Select(assessmentTypeGroup => new AssessmentTypeScores
                        {
                            Name = assessmentTypeGroup.Key,
                            Scores = assessmentTypeGroup.ToList()
                        })
                        .ToList()
                })
                .ToList();

            return areas;
        }

        private bool IsScoreAccepted(UserScore userScore)
        {
            return userScore.Roles.Any(userRole =>
                _config.RoleWeights.Any(configRole => string.Equals(userRole, configRole.Name, StringComparison.OrdinalIgnoreCase))
            );
        }
    }
}
