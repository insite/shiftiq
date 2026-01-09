namespace Shift.Toolbox.Reporting.PerformanceReport.Models
{
    public class AssessmentTypeScore
    {
        public string Name { get; set; }
        public decimal Weight { get; set; }
        public RoleScore[] RoleScores { get; set; }

        public decimal GetWeightedScore()
        {
            var scoreSum = 0m;
            var maxScoreSum = 0m;

            foreach (var role in RoleScores)
            {
                scoreSum += role.WeightedScore;
                maxScoreSum += role.WeightedMaxScore;
            }

            return maxScoreSum > 0 ? scoreSum / maxScoreSum : 0;
        }
    }
}
