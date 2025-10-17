using System;
using System.Linq;

namespace Shift.Toolbox.Reporting.PerformanceReport.Models
{
    public class AreaScore
    {
        public Guid AreaId { get; set; }
        public AssessmentTypeScore[] AssessmentTypeScores { get; set; }

        public decimal GetWeightedScore()
        {
            return AssessmentTypeScores.Sum(x => x.Weight * x.GetWeightedScore());
        }

        public (decimal Weighted, decimal Unweighted) GetScores()
        {
            decimal weighted = 0;
            decimal unweighted = 0;

            foreach (var s in AssessmentTypeScores)
            {
                var weightedScore = s.GetWeightedScore();

                weighted += s.Weight * weightedScore;
                unweighted += weightedScore;
            }    

            return (weighted, unweighted);
        }
    }
}
