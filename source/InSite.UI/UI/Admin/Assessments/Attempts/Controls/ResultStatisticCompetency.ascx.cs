using System;

using InSite.Application.Attempts.Read;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class ResultStatisticCompetency : ResultStatisticBase
    {
        public void LoadData(AttemptAnalysis analysis)
        {
            var data = new BellCurveData();
            var attempts = analysis.Attempts;
            var attemptCount = attempts.Count;
            var attemptPassed = 0;

            for (var i = 0; i < attemptCount; i++)
            {
                var attempt = attempts[i];
                
                var questionPoints = 0m;
                var answerPoints = 0m;

                foreach (var q in attempt.Questions)
                {
                    questionPoints += q.QuestionPoints ?? 0;
                    answerPoints += q.AnswerPoints ?? 0;
                }

                decimal attemptScore = 0;
                if(questionPoints > 0)
                    attemptScore = answerPoints > 0 ? (answerPoints / questionPoints) : 0;

                if (analysis.Forms.TryGetValue(attempt.FormIdentifier, out var form) && attemptScore >= form.Specification.Calculation.PassingScore)
                    attemptPassed++;

                data.AddValue(attemptScore);
            }

            data.BindChart(Chart);

            // Stats

            var median = data.GetMedian();
            var mean = attemptCount != 0 ? data.SumValue / attemptCount : 0;
            var standardDeviation = data.GetStandardDeviation();
            var testReliability = analysis.CalculateCronbachAlpha();
            var sem = double.IsNaN(standardDeviation) || double.IsNaN(testReliability)
                ? double.NaN
                : standardDeviation * Math.Sqrt(1 - testReliability);

            AttemptCompletedCount.Text = attemptCount.ToString("n0");
            AttemptPassedCount.Text = attemptPassed.ToString("n0");
            AttemptPassRate.Text = (attemptCount == 0 ? 0M : (decimal)attemptPassed / attemptCount).ToString("p2");
            QuestionCount.Text = analysis.QuestionCount.ToString("n0");

            HighestScore.Text = data.MaxValue >= 0 ? data.MaxValue.ToString("p0") : "N/A";
            LowestScore.Text = data.MinValue < int.MaxValue ? data.MinValue.ToString("p0") : "N/A";

            ResultsMedian.Text = median.ToString("p2");
            ResultsMean.Text = mean.ToString("p2");
            ResultsStandardDeviation.Text = double.IsNaN(standardDeviation) ? "N/A" : standardDeviation.ToString("n2");
            StandardErrorOfMeasurement.Text = double.IsNaN(sem) ? "N/A" : sem.ToString("n2");
        }
    }
}