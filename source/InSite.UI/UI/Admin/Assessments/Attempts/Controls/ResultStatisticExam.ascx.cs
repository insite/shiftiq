using System;
using System.Linq;

using InSite.Application.Attempts.Read;

namespace InSite.Admin.Assessments.Attempts.Controls
{
    public partial class ResultStatisticExam : ResultStatisticBase
    {
        public void LoadData(AttemptAnalysis analysis, QAttemptFilter filter)
        {
            var data = new BellCurveData();
            var attempts = analysis.Attempts;
            var attemptCount = attempts.Count;
            var allQuestions = analysis.Forms.Values.SelectMany(x => x.GetQuestions()).Distinct().ToArray();
            var attemptStarted = 0;
            var attemptPassed = 0;

            {
                var startedFilter = filter.Clone();
                startedFilter.IsCompleted = null;
                startedFilter.IsStarted = true;

                attemptStarted = ServiceLocator.AttemptSearch.CountAttempts(startedFilter);
            }            

            for (var i = 0; i < attemptCount; i++)
            {
                var attempt = attempts[i];
                if (attempt.AttemptIsPassing)
                    attemptPassed++;

                data.AddValue(attempt.AttemptScore ?? 0);
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

            AttemptStartedCount.Text = attemptStarted.ToString("n0");
            AttemptCompletedCount.Text = attemptCount.ToString("n0");
            AttemptPassedCount.Text = attemptPassed.ToString("n0");
            AttemptPassRate.Text = (attemptCount == 0 ? 0M : (decimal)attemptPassed / attemptCount).ToString("p2");
            QuestionCount.Text = allQuestions.Length.ToString("n0");
            FormPoints.Text = allQuestions.Sum(x => x.Points ?? 0).ToString("n0");

            HighestScore.Text = data.MaxValue >= 0 ? data.MaxValue.ToString("p0") : "N/A";
            LowestScore.Text = data.MinValue < int.MaxValue ? data.MinValue.ToString("p0") : "N/A";

            ResultsMedian.Text = median.ToString("p2");
            ResultsMean.Text = mean.ToString("p2");
            ResultsStandardDeviation.Text = double.IsNaN(standardDeviation) ? "N/A" : standardDeviation.ToString("n2");
            StandardErrorOfMeasurement.Text = double.IsNaN(sem) ? "N/A" : sem.ToString("n2");

            if (!double.IsNaN(testReliability))
            {
                var description = string.Empty;
                if (testReliability < 0.5)
                    description += "Unacceptable";
                else if (testReliability < 0.6)
                    description += "Poor";
                else if (testReliability < 0.7)
                    description += "Questionable";
                else if (testReliability < 0.8)
                    description += "Acceptable";
                else if (testReliability < 0.9)
                    description += "Good";
                else
                    description += "Excellent";

                TestReliability.Text = $"{testReliability:n2} <span class='form-text'>{description}</span>";
            }
            else
            {
                TestReliability.Text = "N/A";
            }
        }
    }
}