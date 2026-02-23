using System.Collections.Generic;

using InSite.Domain.Banks;

namespace InSite.Application.Attempts.Read
{
    public interface IAttemptAnalysisQuestion
    {
        Question Question { get; }

        int AttemptCount { get; }

        int SuccessCount { get; }

        double SuccessRate { get; }

        int NoAnswerCount { get; }

        decimal NoAnswerRate { get; }

        int NoAnswerAverageAttemptScorePercent { get; }

        double NoAnswerItemTotalCorrelation { get; }

        double NoAnswerItemRestCoefficient { get; }

        IEnumerable<IAttemptAnalysisOption> GetOptions();
    }
}
